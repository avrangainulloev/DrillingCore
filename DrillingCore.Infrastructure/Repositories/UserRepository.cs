using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly DrillingCoreDbContext _context;

        public UserRepository(DrillingCoreDbContext context)
        {
            _context = context;
        }

        // Реализация метода аутентификации.
        // Для простоты сравниваем имя и пароль напрямую – в реальном проекте используйте хэширование и безопасное сравнение!
        public async Task<User?> AuthenticateAsync(LoginRequest request)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
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
    }
}
