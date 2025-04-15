using DrillingCore.Application.DTOs;
using DrillingCore.Application.Forms.Commands;
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

        Task SavePhotoAsync(FormPhoto photo);
        Task SaveSignatureAsync(FormSignature signature);
        Task<DrillInspectionDto> GetDrillInspectionByIdAsync(int formId, CancellationToken cancellationToken);
        Task UpdateDrillInspectionAsync(UpdateDrillInspectionCommand command, CancellationToken cancellationToken);
        Task AddProjectFormAsync(ProjectForm form, CancellationToken cancellationToken);


    }


}
