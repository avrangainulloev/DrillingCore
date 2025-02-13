using DrillingCore.Application.DTOs;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
    }
}
