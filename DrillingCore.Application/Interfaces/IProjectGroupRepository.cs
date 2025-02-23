using DrillingCore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IProjectGroupRepository
    {
        Task<IEnumerable<ProjectGroup>> GetByProjectIdAsync(int projectId);
        Task<ProjectGroup?> GetByIdAsync(int groupId);
        Task AddAsync(ProjectGroup group);
        Task DeleteAsync(ProjectGroup group);
        Task<List<ProjectGroup>> GetEmptyGroupsByProjectIdAsync(int projectId);
        Task<ProjectGroup> GetByProjectIdAndNameAsync(int projectId, string groupName);
    }
}
