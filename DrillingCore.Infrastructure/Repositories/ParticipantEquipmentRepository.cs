using DrillingCore.Application.Interfaces;
using DrillingCore.Domain.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class ParticipantEquipmentRepository : IParticipantEquipmentRepository
    {
        private readonly DrillingCoreDbContext _dbContext;

        public ParticipantEquipmentRepository(DrillingCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ParticipantEquipment?> GetByIdAsync(int id)
        {
            return await _dbContext.ParticipantEquipments
                .Include(pe => pe.Participant)
                .ThenInclude(p => p.User) // если нужно имя пользователя
                .Include(pe => pe.Equipment)
                .FirstOrDefaultAsync(pe => pe.Id == id);
        }

        public async Task<IEnumerable<ParticipantEquipment>> GetAllAsync()
        {
            return await _dbContext.ParticipantEquipments
                .Include(pe => pe.Participant)
                .ThenInclude(p => p.User)
                .Include(pe => pe.Equipment)
                .ToListAsync();
        }

        public async Task AddAsync(ParticipantEquipment assignment)
        {
            await _dbContext.ParticipantEquipments.AddAsync(assignment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParticipantEquipment assignment)
        {
            _dbContext.ParticipantEquipments.Update(assignment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(ParticipantEquipment assignment)
        {
            _dbContext.ParticipantEquipments.Remove(assignment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
