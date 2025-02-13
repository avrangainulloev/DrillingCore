using DrillingCore.Application.DTOs;
using DrillingCore.Application.Projects.Groups.Commands;
using DrillingCore.Application.Projects.Participants.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Projects/{projectId}/Groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Projects/{projectId}/Groups
        [HttpGet]
        public async Task<IActionResult> GetGroups(int projectId)
        {
            var query = new GetProjectGroupsQuery(projectId);
            IEnumerable<ProjectGroupDto> groups = await _mediator.Send(query);
            return Ok(groups);
        }

        // POST: api/Projects/{projectId}/Groups
        [HttpPost]
        public async Task<IActionResult> CreateGroup(int projectId, [FromBody] CreateProjectGroupCommand command)
        {
            if (projectId != command.ProjectId)
            {
                return BadRequest("Project ID mismatch");
            }
            int newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetGroups), new { projectId = projectId }, new { id = newId });
        }

        // Можно добавить POST для создания новой группы, если требуется.
    }
}
