using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IFormDeliveryService
    {
        Task TrySendOnFormCompleted(int formId, CancellationToken ct);
        Task TrySendOnAllParticipantsSigned(int projectId, int formTypeId, DateOnly dateFilled, FormDeliveryRule rule, CancellationToken cancellationToken);
        Task<bool> CheckIfAllProjectParticipantsSigned(int projectId, int formTypeId, DateOnly dateFilled, CancellationToken cancellationToken);
    }
}
