using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DrillingCoreDbContext _context;

        public ReportRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectForm>> GetSignedFormsForUserAsync(int userId, DateOnly from, DateOnly to, CancellationToken ct)
        {
            return await _context.FormSignatures
     .Include(sig => sig.ProjectForm)
         .ThenInclude(pf => pf.Project)
     .Include(sig => sig.ProjectForm)
         .ThenInclude(pf => pf.Creator)
     .Where(sig =>
         sig.Participant.UserId == userId &&
         sig.ProjectForm.DateFilled >= from &&
         sig.ProjectForm.DateFilled <= to &&
         sig.ProjectForm.Status == "Completed")
     .Select(sig => sig.ProjectForm)
     .ToListAsync(ct);
        }

        public async Task<List<DrillingForm>> GetDrillingFormsWithUserAsync(int userId, DateOnly from, DateOnly to, CancellationToken ct)
        {
            return await _context.DrillingForms
                .Where(df =>
                    df.ProjectForm.DateFilled >= from &&
                    df.ProjectForm.DateFilled <= to &&
                    df.ProjectForm.FormParticipants.Any(p => p.Participant.UserId == userId))
                .Include(df => df.ProjectForm)
                    .ThenInclude(f => f.Project)
                .ToListAsync(ct);
        }

        public async Task<Participant?> GetActiveParticipantForUserAsync(int userId, DateOnly date, CancellationToken ct)
        {
            return await _context.Participants
                .Where(p =>
                    p.UserId == userId &&
                    p.StartDate <= date &&
                    (p.EndDate == null || p.EndDate >= date))
                .FirstOrDefaultAsync(ct);
        }
    }
}
