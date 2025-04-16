using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class FlhaRepository : IFlhaRepository
    {
        private readonly DrillingCoreDbContext _context;

        public FlhaRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<FLHAHazard>> GetHazardsByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            return await _context.FLHAHazards
                .Where(h => h.GroupId == groupId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddFlhaFormAsync(FLHAForm form, CancellationToken cancellationToken)
        {
            _context.FLHAForms.Add(form);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FLHAForm>> GetFLHAFormsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.FLHAForms
                .Include(f => f.ProjectForm)
                    .ThenInclude(p => p.Creator)
                .Where(f => f.ProjectForm.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateFLHAFormAsync(int formId, FLHAFormCreateDto dto, CancellationToken cancellationToken)
        {
            var flha = await _context.FLHAForms
                .Include(f => f.Hazards)
                .Include(f => f.ProjectForm)
                    .ThenInclude(p => p.FormParticipants)
                .Include(f => f.ProjectForm.FormSignatures)
                .FirstOrDefaultAsync(f => f.ProjectFormId == formId, cancellationToken);

            if (flha == null)
                throw new Exception($"FLHA form with ProjectFormId {formId} not found.");

            var projectForm = flha.ProjectForm;
            projectForm.DateFilled = dto.DateFilled;
            projectForm.OtherComments = dto.OtherComments;
            projectForm.UpdateAt = DateTime.UtcNow;

            // 🔄 Участники — поэлементно
            var existingParticipantIds = projectForm.FormParticipants.Select(p => p.ParticipantId).ToList();
            var newParticipantIds = dto.Participants.Select(p => p.ParticipantId).ToList();

            var participantList = projectForm.FormParticipants.ToList();

            // Удалить тех, кого больше нет
            foreach (var toRemove in participantList.Where(p => !newParticipantIds.Contains(p.ParticipantId)))
            {
                projectForm.FormParticipants.Remove(toRemove);
            }

            // Добавить новых
            foreach (var pid in newParticipantIds.Except(existingParticipantIds))
            {
                projectForm.FormParticipants.Add(new FormParticipant
                {
                    ProjectFormId = projectForm.Id,
                    ParticipantId = pid
                });
            }

            //// 🔄 Подписи — поэлементно
            //foreach (var p in dto.Participants)
            //{
            //    if (!string.IsNullOrEmpty(p.SignatureUrl))
            //    {
            //        var existingSignature = projectForm.FormSignatures
            //            .FirstOrDefault(s => s.ParticipantId == p.ParticipantId);

            //        if (existingSignature == null)
            //        {
            //            // Добавить новую подпись
            //            projectForm.FormSignatures.Add(new FormSignature
            //            {
            //                ProjectFormId = projectForm.Id,
            //                ParticipantId = p.ParticipantId,
            //                SignatureUrl = p.SignatureUrl,
            //                CreatedDate = DateTime.UtcNow
            //            });
            //        }
            //        else if (existingSignature.SignatureUrl != p.SignatureUrl)
            //        {
            //            // Обновить существующую подпись
            //            existingSignature.SignatureUrl = p.SignatureUrl;
            //            existingSignature.CreatedDate = DateTime.UtcNow;
            //        }
            //    }
            //}

            // 📋 Hazards — очистить и добавить заново (опционально оставить как есть)
            flha.TaskDescription = dto.TaskDescription;
            flha.Hazards.Clear();
            foreach (var h in dto.Hazards)
            {
                flha.Hazards.Add(new FLHAFormHazard
                {
                    HazardText = h.HazardText,
                    ControlMeasures = h.ControlMeasures,
                    HazardTemplateId = h.HazardTemplateId
                });
            }

           

            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task<FLHAForm> GetFLHAFormByIdAsync(int formId, CancellationToken cancellationToken)
        {
            return await _context.FLHAForms
                .Include(f => f.ProjectForm)
                    .ThenInclude(p => p.Creator)
                .Include(f => f.ProjectForm.FormParticipants)
                .Include(f => f.ProjectForm.FormPhotos)
                .Include(f => f.ProjectForm.FormSignatures)
                .Include(f => f.Hazards)
                .FirstOrDefaultAsync(f => f.ProjectFormId == formId, cancellationToken)
                ?? throw new Exception($"FLHA form with ID {formId} not found.");
        }

    }
}
