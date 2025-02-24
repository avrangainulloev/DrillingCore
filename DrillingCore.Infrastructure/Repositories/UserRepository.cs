// UserRepository.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DrillingCoreDbContext _context;

        public UserRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(LoginRequest request)
        {
            return await _context.Users
         .Include(u => u.Role)
         .FirstOrDefaultAsync(u =>
             u.Username == request.Username && u.PasswordHash == request.Password);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string searchTerm, int? roleId)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(lowerTerm) ||
                    u.Username.ToLower().Contains(lowerTerm) ||
                    u.Email.ToLower().Contains(lowerTerm));
            }
            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
