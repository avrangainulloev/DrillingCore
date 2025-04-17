using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Repositories
{
    public class FormDeliveryRepository : IFormDeliveryRepository
    {
        private readonly DrillingCoreDbContext _context;

        public FormDeliveryRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task AddRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken)
        {
            _context.FormDeliveryRules.Add(rule);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FormDeliveryRule>?> GetRulesAsync(int projectId, int formTypeId, CancellationToken cancellationToken)
        {
            return await _context.FormDeliveryRules
          .Include(r => r.Recipients)
          .Where(r => r.ProjectId == projectId && r.FormTypeId == formTypeId)
          .ToListAsync(cancellationToken);
        }

        public async Task<List<FormDeliveryRule>> GetRulesByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            return await _context.FormDeliveryRules
                .Where(r => r.ProjectId == projectId)
                .Include(r => r.FormType)
                .Include(r => r.Recipients)
                .ToListAsync(cancellationToken);
        }

        public async Task<FormDeliveryRule?> GetRuleByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.FormDeliveryRules
                .Include(r => r.Recipients)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task UpdateRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken)
        {
            _context.FormDeliveryRules.Update(rule);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken)
        {
            _context.FormDeliveryRules.Remove(rule);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
