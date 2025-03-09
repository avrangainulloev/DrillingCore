using DrillingCore.Application.DTOs;
using DrillingCore.Application.Users.Commands;
using DrillingCore.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        // GET: api/Users?searchTerm=...&roleId=...
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            IEnumerable<UserDto> users = await _mediator.Send(query);
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var query = new GetUserQuery(id);
            UserDto user = await _mediator.Send(query);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            int newUserId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUser), new { id = newUserId }, new { id = newUserId });
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            await _mediator.Send(command);
            return Ok("User updated successfully.");
        }

        // DELETE: api/Users/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    await _mediator.Send(new DeleteUserCommand(id));
        //    return Ok("User deleted successfully.");
        //}
    }
}
