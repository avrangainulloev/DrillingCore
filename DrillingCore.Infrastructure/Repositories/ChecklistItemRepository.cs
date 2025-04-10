using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class ChecklistItemRepository : IChecklistRepository
    {
        private readonly DrillingCoreDbContext _context;

        public ChecklistItemRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChecklistItemDto>> GetByFormTypeAsync(int formTypeId)
        {
            return await _context.ChecklistItems
                .Where(i => i.FormTypeId == formTypeId)
                .Select(i => new ChecklistItemDto
                {
                    Id = i.Id,
                    Label = i.Label,
                    GroupName = i.GroupName
                })
                .ToListAsync();
        }
    }
}
