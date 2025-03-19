using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IEquipmentTypeRepository
    {
        Task<EquipmentType?> GetByIdAsync(int id);
        Task<IEnumerable<EquipmentType>> GetAllAsync();
        Task AddAsync(EquipmentType equipmentType);
        Task UpdateAsync(EquipmentType equipmentType);
        Task DeleteAsync(EquipmentType equipmentType);
    }
}
