using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateAsync(LoginRequest request);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        // Дополнительные методы по необходимости (например, получение пользователя по email)
    }
}
