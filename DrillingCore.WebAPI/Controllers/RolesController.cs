using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Roles.Queries;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Roles")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var query = new GetRolesQuery();
            IEnumerable<RoleDto> roles = await _mediator.Send(query);
            return Ok(roles);
        }
    }
}
