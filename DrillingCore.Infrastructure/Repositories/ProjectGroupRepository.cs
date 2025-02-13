using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence
{
    public class ProjectGroupRepository : IProjectGroupRepository
    {
        private readonly DrillingCoreDbContext _context;

        public ProjectGroupRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectGroup group)
        {
            _context.ProjectGroups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProjectGroup group)
        {
            _context.ProjectGroups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<ProjectGroup?> GetByIdAsync(int groupId)
        {
            return await _context.ProjectGroups
                .Include(g => g.Participants)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<IEnumerable<ProjectGroup>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ProjectGroups
    .Include(g => g.Participants)
        .ThenInclude(p => p.User)
    .Where(g => g.ProjectId == projectId)
    .ToListAsync();
        }
    }
}
