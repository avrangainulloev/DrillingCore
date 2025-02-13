// Application/Interfaces/IProjectRepository.cs
using DrillingCore.Core.Entities;

namespace DrillingCore.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllAsync(int limit = 30);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Project project);
    }
}
