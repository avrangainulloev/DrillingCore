using DrillingCore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IEquipmentRepository
    {

        Task<Equipment?> GetByIdAsync(int id);
        Task<IEnumerable<Equipment>> GetAllAsync();
        Task AddAsync(Equipment equipment);
        Task UpdateAsync(Equipment equipment);
        Task DeleteAsync(Equipment equipment);
    }
}
