using DrillingCore.Application.DTOs;
using DrillingCore.Domain.Entities;

namespace DrillingCore.Application.Interfaces
{
    public interface IParticipantEquipmentRepository
    {
        Task<IEnumerable<ParticipantEquipment>> GetEquipmentByParticipantIdAsync(int participantId, int projectId, CancellationToken cancellationToken);
        Task<ParticipantEquipment?> GetByIdAsync(int id);
        Task<IEnumerable<ParticipantEquipment>> GetAllAsync();
        Task AddAsync(ParticipantEquipment assignment);
        Task UpdateAsync(ParticipantEquipment assignment);
        Task DeleteAsync(ParticipantEquipment assignment);

        Task<EquipmentForFormDto?> GetParticipantActiveEquipmentByTypeAsync(int participantId, int projectId, int equipmentTypeId);
    }
}
