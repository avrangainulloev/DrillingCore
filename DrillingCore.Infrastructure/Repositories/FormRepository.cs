using DrillingCore.Application.DTOs;
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

        public async Task<List<FormPhotoDto>> GetFormPhotosAsync(int formId)
        {
            return await _context.FormPhotos
                .Where(p => p.ProjectFormId == formId)
                .Select(p => new FormPhotoDto
                {
                    Id = p.Id,
                    PhotoUrl = p.PhotoUrl,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<List<FormSignatureDto>> GetFormSignaturesAsync(int formId)
        {
            return await _context.FormParticipants
               .Where(p => p.ProjectFormId == formId && !string.IsNullOrEmpty(p.Signature))
               .Include(p => p.Participant)
               .ThenInclude(p => p.User)
               .Select(p => new FormSignatureDto
               {
                   ParticipantId = p.ParticipantId,
                   ParticipantName = p.Participant!.User!.FullName,
                   Signature = p.Signature!
               })
               .ToListAsync();
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
        }
    }
}
