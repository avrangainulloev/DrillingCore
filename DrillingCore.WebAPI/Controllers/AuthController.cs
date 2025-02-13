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
    }
}
