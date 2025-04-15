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
        Task TrySendOnFormCompleted(ProjectForm form, CancellationToken ct);
    }
}
