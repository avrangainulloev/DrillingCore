using DrillingCore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetByIdAsync(int id);
        Task AddAsync(Role role);
    }
}
