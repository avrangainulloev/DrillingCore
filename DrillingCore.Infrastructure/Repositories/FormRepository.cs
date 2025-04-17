using DrillingCore.Application.DTOs;
using DrillingCore.Application.Forms.Commands;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly DrillingCoreDbContext _context;
        private readonly IFormDeliveryService _formDeliveryService;
        private readonly IFormDeliveryRepository _formDeliveryRepository;
        private readonly IBackgroundTaskQueue _taskQueue;
        public FormRepository(DrillingCoreDbContext context, IFormDeliveryService formDeliveryService, IFormDeliveryRepository formDeliveryRepository, IBackgroundTaskQueue taskQueue)
        {
            _context = context;
            _formDeliveryService = formDeliveryService;
            _formDeliveryRepository = formDeliveryRepository;
            _taskQueue = taskQueue;
        }
        public async Task AddProjectFormAsync(ProjectForm form, CancellationToken cancellationToken)
        {
            _context.ProjectForms.Add(form);

            //var total = form.FormParticipants.Count;
            //var signed = form.FormSignatures.Count;

            //form.Status = (signed >= total && total > 0)
            //    ? "Completed"
            //    : "Pending";


            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<ProjectForm>> GetFormsByProjectAndTypeAsync(int projectId, int formTypeId)
        {
            return await _context.ProjectForms
                .Include(f => f.FormType)
                .Where(f => f.ProjectId == projectId && f.FormTypeId == formTypeId)
                .ToListAsync();
        }

        public async Task<int> CreateDrillInspectionAsync(ProjectForm form,
       List<FormChecklistResponse> checklistResponses,
       List<FormParticipant> participants,
      
       CancellationToken cancellationToken)
        {
            _context.ProjectForms.Add(form);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var item in checklistResponses)
            {
                item.ProjectFormId = form.Id;
            }

            foreach (var item in participants)
            {
                item.ProjectFormId = form.Id;
            }

            

            _context.FormChecklistResponses.AddRange(checklistResponses);
            _context.FormParticipants.AddRange(participants);
            

            await _context.SaveChangesAsync(cancellationToken);
            return form.Id;
        }

        public async Task<int?> GetEquipmentTypeIdForFormTypeAsync(int formTypeId)
        {
            return await _context.FormTypeEquipmentTypes
                .Where(x => x.FormTypeId == formTypeId)
                .Select(x => (int?)x.EquipmentTypeId)

                .FirstOrDefaultAsync();
        }

        public async Task SavePhotoAsync(FormPhoto photo)
        {
            _context.FormPhotos.Add(photo);
            await _context.SaveChangesAsync();
        }

        public async Task SaveSignatureAsync(FormSignature signature, CancellationToken cancellationToken)
        {
            // 1. Сохранение или обновление подписи
            var existing = await _context.FormSignatures
                .FirstOrDefaultAsync(s =>
                    s.ProjectFormId == signature.ProjectFormId &&
                    s.ParticipantId == signature.ParticipantId,
                    cancellationToken);

            if (existing == null)
            {
                _context.FormSignatures.Add(signature);
            }
            else
            {
                existing.SignatureUrl = signature.SignatureUrl;
                existing.CreatedDate = DateTime.UtcNow;
                _context.FormSignatures.Update(existing);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // 2. Загрузка формы с зависимостями
            var projectForm = await _context.ProjectForms
                .Include(p => p.Project)
                .Include(p => p.FormType)
                .Include(p => p.FormParticipants).ThenInclude(fp => fp.Participant).ThenInclude(p => p.User)
                .Include(p => p.FormSignatures)
                .Include(p => p.FormChecklistResponses).ThenInclude(r => r.ChecklistItem)
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.Id == signature.ProjectFormId, cancellationToken);

            if (projectForm == null)
                return;

            // 3. Подсчёт подписей
            var total = projectForm.FormParticipants.Count;
            var signed = projectForm.FormSignatures
                .Count(s => projectForm.FormParticipants.Any(p => p.ParticipantId == s.ParticipantId));

            projectForm.Status = (signed >= total && total > 0)
                ? "Completed"
                : "Pending";

            await _context.SaveChangesAsync(cancellationToken);

            // 4. Обработка правил доставки
            if (projectForm.Status == "Completed")
            {
                // Загружаем правило
                var rules = await _formDeliveryRepository.GetRulesAsync(
                    projectForm.ProjectId,
                    projectForm.FormTypeId,
                    cancellationToken);
                foreach (var rule in rules)
                {
                    if (rule?.Condition == FormDeliveryCondition.AfterEachFormCompleted)
                    {
                        await _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
                        {
                            var deliveryService = sp.GetRequiredService<IFormDeliveryService>();
                            await deliveryService.TrySendOnFormCompleted(projectForm.Id, rule, token);
                        });

                    }
                    else if (rule?.Condition == FormDeliveryCondition.AfterAllParticipantsSigned)
                    {
                        var allSigned = await _formDeliveryService.CheckIfAllProjectParticipantsSigned(
                            projectForm.ProjectId,
                            projectForm.FormTypeId,
                            projectForm.DateFilled,

                            cancellationToken);

                        if (allSigned)
                        {
                            await _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
                            {
                                var deliveryService = sp.GetRequiredService<IFormDeliveryService>();
                                await deliveryService.TrySendOnAllParticipantsSigned(
                                projectForm.ProjectId,
                                projectForm.FormTypeId,
                                projectForm.DateFilled,
                                rule,
                                cancellationToken);
                            });
                        }
                    }
                }
            }
        }

        public async Task<DrillInspectionDto> GetDrillInspectionByIdAsync(int formId, CancellationToken cancellationToken)
        {
            var form = await _context.ProjectForms
                .Include(f => f.FormChecklistResponses)
                .Include(f => f.FormParticipants)
                .Include(f => f.FormPhotos)
                .Include(f => f.FormSignatures)
                .FirstOrDefaultAsync(f => f.Id == formId, cancellationToken);

            if (form == null) throw new Exception("Form not found");

            return new DrillInspectionDto
            {
                Id = form.Id,
                CrewName = form.CrewName,
                UnitNumber = form.UnitNumber,
                DateFilled = form.DateFilled,
                OtherComments = form.OtherComments,
                ProjectId = form.ProjectId,
                ChecklistResponses = form.FormChecklistResponses.Select(x => new ChecklistResponseDto
                {
                    ChecklistItemId = x.ChecklistItemId,
                    Response = x.Response
                }).ToList(),
                Participants = form.FormParticipants.Select(p => new FormParticipantDto
                {
                    ParticipantId = p.ParticipantId,
                    Signature = p.Signature
                }).ToList(),
                PhotoUrls = form.FormPhotos.Select(p => p.PhotoUrl).ToList(),
                Signatures = form.FormSignatures.Select(s => new FormSignatureDto
                {
                    ParticipantId = s.ParticipantId,
                    SignatureUrl = s.SignatureUrl
                }).ToList()
            };
        }

        public async Task UpdateDrillInspectionAsync(UpdateDrillInspectionCommand command, CancellationToken cancellationToken)
        {
            var form = await _context.ProjectForms
                .Include(f => f.FormChecklistResponses)
                .Include(f => f.FormParticipants)
                .FirstOrDefaultAsync(f => f.Id == command.FormId, cancellationToken);

            if (form == null)
                throw new Exception("Form not found");

            form.CrewName = command.CrewName;
            form.UnitNumber = command.UnitNumber;
            form.DateFilled = command.DateFilled;
            form.OtherComments = command.OtherComments;
            form.UpdateAt = DateTime.UtcNow;

            // Обновим чеклисты
            _context.FormChecklistResponses.RemoveRange(form.FormChecklistResponses);
            _context.FormChecklistResponses.AddRange(command.ChecklistResponses.Select(c => new FormChecklistResponse
            {
                ProjectFormId = form.Id,
                ChecklistItemId = c.ChecklistItemId,
                Response = c.Response
            }));

            // Обновим участников
            _context.FormParticipants.RemoveRange(form.FormParticipants);
            _context.FormParticipants.AddRange(command.Participants.Select(p => new FormParticipant
            {
                ProjectFormId = form.Id,
                ParticipantId = p.ParticipantId
            }));

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FormType>> GetFormTypesAsync()
        {
            return await _context.FormTypes.ToListAsync();
        }

        public async Task<int> CreateDrillingFormAsync(ProjectForm form, DrillingForm drillingForm, List<FormParticipant> participants, CancellationToken cancellationToken)
        {
            _context.ProjectForms.Add(form);
            await _context.SaveChangesAsync(cancellationToken); // Чтобы получить form.Id

            drillingForm.ProjectFormId = form.Id;
            _context.DrillingForms.Add(drillingForm);

            foreach (var participant in participants)
                participant.ProjectFormId = form.Id;

            _context.FormParticipants.AddRange(participants);

            await _context.SaveChangesAsync(cancellationToken);
            return form.Id;
        }

        public async Task<ProjectForm?> GetDrillingFormWithDetailsAsync(int formId, CancellationToken ct)
        {
            return await _context.ProjectForms
                .Include(f => f.DrillingForm)
                .Include(f => f.FormParticipants)
                .Include(f => f.FormPhotos)
                .Include(f => f.FormSignatures)
                .Include(f => f.FormType)
                .Include(f => f.Creator)
                .Include(f => f.Project)
                .FirstOrDefaultAsync(f => f.Id == formId, ct);
        }

        public async Task UpdateDrillingFormAsync(UpdateDrillingFormCommand command, CancellationToken cancellationToken)
        {
            var form = await _context.ProjectForms
                .Include(f => f.DrillingForm)
                .Include(f => f.FormParticipants)
                .FirstOrDefaultAsync(f => f.Id == command.FormId, cancellationToken);

            if (form == null || form.DrillingForm == null)
                throw new Exception("Form not found");

            form.DateFilled = command.DateFilled;
            form.OtherComments = command.OtherComments;
            form.UpdateAt = DateTime.UtcNow;
           

            form.DrillingForm.NumberOfWells = command.TotalWells;
            form.DrillingForm.TotalMeters = command.TotalMeters;

            
            // Обновим участников
            _context.FormParticipants.RemoveRange(form.FormParticipants);
            _context.FormParticipants.AddRange(command.Participants.Select(p => new FormParticipant
            {
                ProjectFormId = form.Id,
                ParticipantId = p.ParticipantId
            }));

            await _context.SaveChangesAsync(cancellationToken);
        }

        // Infrastructure/Repositories/FormRepository.cs
        public async Task<DrillingFormFullDto> GetDrillingFormByIdAsync(int formId, CancellationToken cancellationToken)
        {
            var form = await _context.ProjectForms
                .Include(f => f.DrillingForm)
                .Include(f => f.FormParticipants)
                .Include(f => f.FormPhotos)
                .Include(f => f.FormSignatures)
                .FirstOrDefaultAsync(f => f.Id == formId, cancellationToken);

            if (form == null || form.DrillingForm == null)
                throw new Exception("Drilling form not found");

            return new DrillingFormFullDto
            {
                Id = form.Id,
                ProjectId = form.ProjectId,
                DateFilled = form.DateFilled,
                TotalWells = form.DrillingForm.NumberOfWells,
                TotalMeters = form.DrillingForm.TotalMeters,
                OtherComments = form.OtherComments,
                Participants = form.FormParticipants.Select(p => new FormParticipantDto
                {
                    ParticipantId = p.ParticipantId,
                    Signature = p.Signature
                }).ToList(),
                PhotoUrls = form.FormPhotos.Select(p => p.PhotoUrl).ToList(),
                Signatures = form.FormSignatures.Select(s => new FormSignatureDto
                {
                    ParticipantId = s.ParticipantId,
                    SignatureUrl = s.SignatureUrl
                }).ToList()
            };
        }

        public async Task<List<DrillingForm>> GetDrillingFormsByProjectAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.DrillingForms
                .Include(f => f.ProjectForm)
                .ThenInclude(pf=>pf.Creator)
                .Include(f=>f.ProjectForm.Creator)
                .Where(f => f.ProjectForm.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

    }
}
