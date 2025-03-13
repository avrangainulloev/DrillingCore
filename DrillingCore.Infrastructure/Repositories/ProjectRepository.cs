// Infrastructure/Repositories/ProjectRepository.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task<IEnumerable<ProjectGroupDto>> GetProjectGroupsWithParticipantsAsync(int projectId)
        {
            // 1. Запрос: группы с участниками (даже если группа пустая)
            var groupsQuery = _dbContext.ProjectGroups
                .Where(g => g.ProjectId == projectId)
                .SelectMany(g => g.Participants.DefaultIfEmpty(), (g, p) => new
                {
                    GroupId = (int?)g.Id,
                    GroupName = g.GroupName,
                    ParticipantId = p == null ? (int?)null : (int?)p.Id,
                    ProjectId = projectId,
                    UserId = p == null ? (int?)null : (int?)p.UserId,
                    DateAdded = p == null ? (DateTime?)null : (DateTime?)p.DateAdded,
                    EndDate = p == null ? (DateTime?)null : (DateTime?)p.EndDate,
                    FullName = p == null ? null : p.User.FullName,
                    Mobile = p == null ? null : p.User.Mobile,
                    Email = p == null ? null : p.User.Email,
                    Role = p == null ? null : p.User.Role.Name
                });

            // 2. Запрос: участники без группы (GroupId == null)
            var ungroupedQuery = _dbContext.Participants
                .Where(p => p.ProjectId == projectId && p.GroupId == null)
                .Select(p => new
                {
                    GroupId = (int?)null,
                    GroupName = "No Group",
                    ParticipantId = (int?)p.Id,
                    ProjectId = projectId,
                    UserId = (int?)p.UserId,
                    DateAdded = (DateTime?)p.DateAdded,
                    EndDate = (DateTime?)p.EndDate,
                    FullName = p.User.FullName,
                    Mobile = p.User.Mobile,
                    Email = p.User.Email,
                    Role = p.User.Role.Name
                });

            // 3. Запрос: пустые группы (в которых нет участников)
            var emptyGroupsQuery = _dbContext.ProjectGroups
                .Where(g => g.ProjectId == projectId && !g.Participants.Any())
                .Select(g => new
                {
                    GroupId = (int?)g.Id,
                    GroupName = g.GroupName,
                    ParticipantId = (int?)null,
                    ProjectId = projectId,
                    UserId = (int?)null,
                    DateAdded = (DateTime?)null,
                    EndDate = (DateTime?)null,
                    FullName = (string)null,
                    Mobile = (string)null,
                    Email = (string)null,
                    Role = (string)null
                });

            // Объединяем все части через Union
            var unionQuery = groupsQuery.Union(ungroupedQuery).Union(emptyGroupsQuery);

            var data = await unionQuery.ToListAsync();

            // Группировка по ключу (GroupId, GroupName)
            var grouped = data.GroupBy(x => new { x.GroupId, x.GroupName });

            var result = grouped.Select(grp => new ProjectGroupDto
            {
                // Если GroupId равен null – для группы "No Group" используем условное значение 0
                Id = grp.Key.GroupId ?? 0,
                ProjectId = projectId,
                GroupName = grp.Key.GroupName,
                Participants = grp.Where(x => x.ParticipantId.HasValue)
                                  .Select(x => new ParticipantDto
                                  {
                                      Id = x.ParticipantId.Value,
                                      ProjectId = x.ProjectId,
                                      UserId = x.UserId.Value,
                                      GroupId = x.GroupId,
                                      DateAdded = x.DateAdded.Value,
                                      EndDate = x.EndDate,
                                      FullName = x.FullName,
                                      Mobile = x.Mobile,
                                      Email = x.Email,
                                      Role = x.Role
                                  }).ToList()
            }).ToList();

            return result;
        }






    }
}
