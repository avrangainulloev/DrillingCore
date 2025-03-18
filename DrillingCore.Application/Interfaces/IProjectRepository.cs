// Application/Interfaces/IProjectRepository.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;

namespace DrillingCore.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllAsync(int limit, string? searchTerm = null, string? status = null);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Project project);
        Task<IEnumerable<ProjectGroupDto>> GetProjectGroupsWithParticipantsAsync(int projectId);
    }
}
