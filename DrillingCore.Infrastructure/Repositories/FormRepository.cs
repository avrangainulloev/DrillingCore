using DrillingCore.Application.DTOs;
using DrillingCore.Application.Forms.Commands;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly DrillingCoreDbContext _context;

        public FormRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }
        public async Task AddProjectFormAsync(ProjectForm form, CancellationToken cancellationToken)
        {
            _context.ProjectForms.Add(form);

            var total = form.FormParticipants.Count;
            var signed = form.FormSignatures.Count;

            form.Status = (signed >= total && total > 0)
                ? "Completed"
                : "Pending";


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

        public async Task SaveSignatureAsync(FormSignature signature)
        {
            _context.FormSignatures.Add(signature);
            await _context.SaveChangesAsync();
            // Найти ProjectForm
            var projectForm = await _context.ProjectForms
                .Include(p => p.FormParticipants)
                .Include(p => p.FormSignatures)
                .FirstOrDefaultAsync(p => p.Id == signature.ProjectFormId);

            if (projectForm != null)
            {
                var total = projectForm.FormParticipants.Count;
                var signed = projectForm.FormSignatures
                    .Count(s => projectForm.FormParticipants.Any(p => p.ParticipantId == s.ParticipantId));

                projectForm.Status = (signed >= total && total > 0)
                    ? "Completed"
                    : "Pending";

                await _context.SaveChangesAsync();
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

    }
}
