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
        Task UpdateUserAsync(User user);
 
        Task<IEnumerable<User>> GetUsersAsync(string searchTerm, int? roleId);

        Task<IEnumerable<User>> GetAvailableUsersAsync();
        Task<User?> GetByUsernameAsync(string username);
    }
}
