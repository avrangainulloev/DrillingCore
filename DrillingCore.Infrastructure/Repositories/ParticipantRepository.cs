using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly DrillingCoreDbContext _context;

        public ParticipantRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Participant participant)
        {
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Participant participant)
        {
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<Participant?> GetByIdAsync(int id)
        {
            return await _context.Participants.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Participant>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Participants
                .Where(p => p.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Participant participant)
        {
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();
        }
    }
}
