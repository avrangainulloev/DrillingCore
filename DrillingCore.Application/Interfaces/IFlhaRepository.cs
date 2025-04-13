using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IFlhaRepository
    {
        Task<List<FLHAHazard>> GetHazardsByGroupIdAsync(int groupId, CancellationToken cancellationToken);
        Task AddFlhaFormAsync(FLHAForm form, CancellationToken cancellationToken);
    }
}
