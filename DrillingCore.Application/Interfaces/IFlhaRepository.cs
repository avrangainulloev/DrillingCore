using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;

namespace DrillingCore.Application.Interfaces
{
    public interface IFlhaRepository
    {
        Task<List<FLHAHazard>> GetHazardsByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task AddFlhaFormAsync(FLHAForm form, CancellationToken cancellationToken);
        Task<List<FLHAForm>> GetFLHAFormsByProjectIdAsync(int projectId, CancellationToken cancellationToken);
        Task<FLHAForm> GetFLHAFormByIdAsync(int formId, CancellationToken token);
        Task UpdateFLHAFormAsync(int formId, FLHAFormCreateDto dto, CancellationToken token);
    }
}
