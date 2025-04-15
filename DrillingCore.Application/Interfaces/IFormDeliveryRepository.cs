using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IFormDeliveryRepository
    {
        Task AddRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken);
        Task<FormDeliveryRule?> GetRuleAsync(int projectId, int formTypeId, CancellationToken cancellationToken);
        Task<List<FormDeliveryRule>> GetRulesByProjectIdAsync(int projectId, CancellationToken cancellationToken);

        Task<FormDeliveryRule?> GetRuleByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken);
        Task DeleteRuleAsync(FormDeliveryRule rule, CancellationToken cancellationToken);




    }

}
