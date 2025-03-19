using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class EquipmentTypeRepository : IEquipmentTypeRepository
    {
        private readonly DrillingCoreDbContext _dbContext;

        public EquipmentTypeRepository(DrillingCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EquipmentType?> GetByIdAsync(int id)
        {
            return await _dbContext.EquipmentTypes.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<EquipmentType>> GetAllAsync()
        {
            return await _dbContext.EquipmentTypes.ToListAsync();
        }

        public async Task AddAsync(EquipmentType equipmentType)
        {
            await _dbContext.EquipmentTypes.AddAsync(equipmentType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(EquipmentType equipmentType)
        {
            _dbContext.EquipmentTypes.Update(equipmentType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(EquipmentType equipmentType)
        {
            _dbContext.EquipmentTypes.Remove(equipmentType);
            await _dbContext.SaveChangesAsync();
        }
    }
}
