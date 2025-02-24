// IUserRepository.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateAsync(LoginRequest request);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);

        // Новый метод для получения пользователей с фильтрацией
        Task<IEnumerable<User>> GetUsersAsync(string searchTerm, int? roleId);
    }
}
