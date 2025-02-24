using DrillingCore.Application.DTOs;
using DrillingCore.Application.Users.Commands;
using DrillingCore.Application.Users.Queries;
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
        //[HttpPost]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        //{
        //    int newId = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(GetUserById), new { id = newId }, new { id = newId });
        //}

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string? searchTerm, [FromQuery] int? roleId)
        {
            var query = new GetUsersQuery { SearchTerm = searchTerm, RoleId = roleId };
            IEnumerable<UserDto> users = await _mediator.Send(query);
            return Ok(users);
        }
    }
}
