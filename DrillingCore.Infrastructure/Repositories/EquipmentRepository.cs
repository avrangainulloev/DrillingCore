using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities; 
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly DrillingCoreDbContext _dbContext;

        public EquipmentRepository(DrillingCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _dbContext.Equipments
                                   .Include(e => e.EquipmentType)
                                   .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            return await _dbContext.Equipments
                                   .Include(e => e.EquipmentType)
                                   .ToListAsync();
        }

        public async Task AddAsync(Equipment equipment)
        {
            await _dbContext.Equipments.AddAsync(equipment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _dbContext.Equipments.Update(equipment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Equipment equipment)
        {
            _dbContext.Equipments.Remove(equipment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
