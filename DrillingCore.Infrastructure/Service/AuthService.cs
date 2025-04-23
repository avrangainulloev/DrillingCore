using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService; // Сервис для генерации JWT
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            // Получаем пользователя через метод аутентификации репозитория.
            var user = await _userRepository.AuthenticateAsync(request);
            if (user == null)
            {
                return new LoginResponse
                {
                    
                    IsAuthenticated = false,
                    Token = string.Empty,
                    Username = string.Empty,
                    Role = string.Empty
                };
            }

            // Генерируем JWT через ITokenService (реализуйте его по своему усмотрению)
            var token = _tokenService.GenerateToken(user);

            // Устанавливаем JWT в HttpOnly куку
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // в production должно быть true
                Expires = DateTime.UtcNow.AddHours(1),
                SameSite = SameSiteMode.None // Важно
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

            // Возвращаем данные пользователя без токена в теле ответа
            return new LoginResponse
            {
                UserId = user.Id,
                IsAuthenticated = true,
                Token = string.Empty, // Токен не передаётся во фронтенд
                Username = user.Username,
                Role = user.Role.Name,
                RoleId = user.RoleId,
                Fullname = user.FullName

            };
        }

        public async Task<LoginResponse> AuthenticateMobileAsync(LoginRequest request)
        {
            var user = await _userRepository.AuthenticateAsync(request);
            if (user == null)
            {
                return new LoginResponse
                {
                    IsAuthenticated = false,
                    Token = string.Empty,
                    Username = string.Empty,
                    Role = string.Empty
                };
            }

            var token = _tokenService.GenerateToken(user);

            // ❗ Не устанавливаем куку для мобилки

            return new LoginResponse
            {
                UserId = user.Id,
                IsAuthenticated = true,
                Token = token, // ← токен теперь ВОЗВРАЩАЕМ в теле
                Username = user.Username,
                Role = user.Role.Name
            };
        }
    }
}
