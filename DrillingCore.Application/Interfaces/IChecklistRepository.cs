using DrillingCore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IChecklistRepository
    {
        Task<List<ChecklistItemDto>> GetByFormTypeAsync(int formTypeId);
    }
}
