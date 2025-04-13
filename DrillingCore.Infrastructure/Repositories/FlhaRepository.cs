using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class FlhaRepository : IFlhaRepository
    {
        private readonly DrillingCoreDbContext _context;

        public FlhaRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<FLHAHazard>> GetHazardsByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            return await _context.FLHAHazards
                .Where(h => h.GroupId == groupId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddFlhaFormAsync(FLHAForm form, CancellationToken cancellationToken)
        {
            _context.FLHAForms.Add(form);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
