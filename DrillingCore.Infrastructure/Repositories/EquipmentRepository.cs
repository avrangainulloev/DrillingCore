using DrillingCore.Application.DTOs;
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

        public async Task<IEnumerable<Equipment>> GetAllAsync(string? searchTerm, int? equipmentTypeId, int limit, CancellationToken cancellationToken)
        {
            var query = _dbContext.Equipments
                                  .Include(e => e.EquipmentType)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string term = searchTerm.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(term) || e.RegistrationNumber.ToLower().Contains(term));
            }

            if (equipmentTypeId.HasValue)
            {
                query = query.Where(e => e.EquipmentTypeId == equipmentTypeId.Value);
            }

            return await query.OrderByDescending(e => e.CreatedDate)
                              .Take(limit)
                              .ToListAsync(cancellationToken);
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
