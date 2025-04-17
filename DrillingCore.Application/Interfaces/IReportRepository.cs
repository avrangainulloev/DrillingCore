using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<List<ProjectForm>> GetSignedFormsForUserAsync(int userId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
        Task<List<DrillingForm>> GetDrillingFormsWithUserAsync(int userId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
        Task<Participant?> GetActiveParticipantForUserAsync(int userId, DateOnly date, CancellationToken cancellationToken);
    }
}
