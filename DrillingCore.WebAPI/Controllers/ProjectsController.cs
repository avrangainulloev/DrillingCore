// Presentation/Controllers/ProjectsController.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Groups.Queries;
using DrillingCore.Application.Projects;
using DrillingCore.Application.Projects.Commands;
using DrillingCore.Application.Projects.Queries;
using DrillingCore.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> GetProjects(
      [FromQuery] int limit = 30,
      [FromQuery] string? searchTerm = null,
      [FromQuery] string? status = null)
        {
            var query = new GetProjectsQuery
            {
                Limit = limit,
                SearchTerm = searchTerm,
                Status = status
            };
            var projects = await _mediator.Send(query);
            return Ok(projects);
        }
        // GET: api/Projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var query = new GetProjectByIdQuery { Id = id };
            var project = await _mediator.Send(query);
            if (project == null)
                return NotFound();
            return Ok(project);
        }
        // POST: api/Projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateProjectCommand { ProjectDto = projectDto };
            var projectId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectId }, new { id = projectId });
        }

        // PUT: api/Projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            if (id != projectDto.Id)
            {
                return BadRequest("Project ID in URL does not match ID in body");
            }

            // Отправляем команду на обновление проекта
            await _mediator.Send(new UpdateProjectCommand { ProjectDto = projectDto });
            return NoContent();
        }

       


    }
}
