using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DrillingCoreDbContext _context;
        public RoleRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task AddAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }
    }
}
