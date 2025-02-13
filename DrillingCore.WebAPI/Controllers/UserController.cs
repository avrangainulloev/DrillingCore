using DrillingCore.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            int newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserById), new { id = newId }, new { id = newId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            // Реализация получения пользователя (может использовать IUserRepository)
            return Ok();
        }
    }
}
