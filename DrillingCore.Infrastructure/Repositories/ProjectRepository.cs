// Infrastructure/Repositories/ProjectRepository.cs
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DrillingCoreDbContext _dbContext;

        public ProjectRepository(DrillingCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync(int limit = 30)
        {
            return await _dbContext.Projects
                                   .OrderByDescending(p => p.StartDate)
                                   .Take(limit)
                                   .ToListAsync();
        }

        public async Task AddAsync(Project project)
        {
            await _dbContext.Projects.AddAsync(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _dbContext.Projects.Update(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Project project)
        {
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
        }
    }
}
