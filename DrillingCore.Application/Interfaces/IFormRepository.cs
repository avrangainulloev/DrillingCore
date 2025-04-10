using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IFormRepository
    {
        Task<List<ProjectForm>> GetFormsByProjectAndTypeAsync(int projectId, int formTypeId);

        Task<int> CreateDrillInspectionAsync(ProjectForm form,
     List<FormChecklistResponse> checklistResponses,
     List<FormParticipant> participants,
    
     CancellationToken cancellationToken);

        Task<int?> GetEquipmentTypeIdForFormTypeAsync(int formTypeId);

        Task<List<FormPhotoDto>> GetFormPhotosAsync(int formId);
        Task<List<FormSignatureDto>> GetFormSignaturesAsync(int formId);

        Task SavePhotoAsync(FormPhoto photo);
        Task SaveSignatureAsync(FormSignature signature);
    }


}
