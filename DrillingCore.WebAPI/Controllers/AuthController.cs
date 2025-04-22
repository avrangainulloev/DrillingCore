using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService userService)
        {
            _authService = userService;
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    var response = await _authService.AuthenticateAsync(request);
        //    if (response == null)
        //        return Unauthorized("Неверный логин или пароль");
        //    return Ok(response);
        //}



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.AuthenticateAsync(request);
            if (!response.IsAuthenticated)
                return Unauthorized(response);
            return Ok(response);
        }

        /// <summary>
        /// Mobile login endpoint — используется мобильным приложением (MAUI).
        /// </summary>
        /// <param name="request">Данные для входа (имя пользователя и пароль).</param>
        /// <returns>
        /// Возвращает <see cref="LoginResponse"/> с полями:
        /// <list type="bullet">
        /// <item><description><c>IsAuthenticated</c> — флаг успешной аутентификации</description></item>
        /// <item><description><c>UserId</c>, <c>Username</c>, <c>Role</c></description></item>
        /// <item><description><c>Token</c> — JWT-токен для использования в заголовке Authorization</description></item>
        /// </list>
        /// При неудачной попытке входа возвращает 401 Unauthorized.
        /// </returns>
        [HttpPost("mobile-login")]
        public async Task<IActionResult> MobileLogin([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateMobileAsync(request);

            if (!result.IsAuthenticated)
                return Unauthorized();

            return Ok(result); // возвращаем token в теле (для MAUI)
        }

    }
}
