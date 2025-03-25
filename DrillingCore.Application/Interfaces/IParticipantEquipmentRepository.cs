using DrillingCore.Domain.Entities;

namespace DrillingCore.Application.Interfaces
{
    public interface IParticipantEquipmentRepository
    {
        Task<IEnumerable<ParticipantEquipment>> GetByParticipantIdAsync(int participantId, CancellationToken cancellationToken);
        Task<ParticipantEquipment?> GetByIdAsync(int id);
        Task<IEnumerable<ParticipantEquipment>> GetAllAsync();
        Task AddAsync(ParticipantEquipment assignment);
        Task UpdateAsync(ParticipantEquipment assignment);
        Task DeleteAsync(ParticipantEquipment assignment);
    }
}
