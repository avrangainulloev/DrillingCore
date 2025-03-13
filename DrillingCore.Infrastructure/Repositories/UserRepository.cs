// UserRepository.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Exceptions;
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

        // Новый метод для обновления пользователя
        public async Task UpdateUserAsync(User user)
        {
            // Находим существующего пользователя по идентификатору
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Обновляем свойства пользователя
            existingUser.Username = user.Username;
            // Если не требуется обновлять пароль, можно исключить PasswordHash,
            // либо обновлять его только при необходимости (например, при сбросе пароля)
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Mobile = user.Mobile;
            existingUser.RoleId = user.RoleId;
            // Если добавлено новое поле IsActive:
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAvailableUsersAsync()
        {
            // Предполагается, что таблица Participants содержит участников проектов,
            // у которых EndDate = null означает, что они сейчас работают
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive && !_context.Participants.Any(p => p.UserId == u.Id && p.EndDate == null))
                .ToListAsync();
        }
    }
}
