using DrillingCore.Application.DTOs;
using DrillingCore.Application.FLHA.Commands;
using DrillingCore.Application.FLHA.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/flha")]
    public class FLHAController : Controller
    {
        private readonly IMediator _mediator;

        public FLHAController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("hazards/{groupId}")]
        public async Task<IActionResult> GetHazardsByGroupId(int groupId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetHazardsByGroupIdQuery { GroupId = groupId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new FLHA form.
        /// </summary>
        /// <param name="dto">FLHA form data.</param>
        /// <returns>The ID of the newly created form.</returns>
        /// <response code="200">Form created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFLHA([FromBody] FLHAFormCreateDto dto)
        {
            var formId = await _mediator.Send(new CreateFLHAFormCommand { Dto = dto });
            return Ok(new { formId });
        }

        /// <summary>
        /// Returns all FLHA forms for a given project.
        /// </summary>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<FLHAFormListDto>), 200)]
        public async Task<IActionResult> GetFormsByProject(int projectId, CancellationToken cancellationToken)
        {
            var forms = await _mediator.Send(new GetFLHAFormsByProjectIdQuery { ProjectId = projectId }, cancellationToken);
            return Ok(forms);
        }

        /// <summary>
        /// Получает FLHA форму по её ID.
        /// </summary>
        /// <param name="formId">ID формы.</param>
        /// <returns>Полная информация о форме.</returns>
        [HttpGet("{formId}")]
        [ProducesResponseType(typeof(FLHAFormDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int formId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFLHAFormByIdQuery { FormId = formId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Обновляет существующую FLHA форму.
        /// </summary>
        /// <param name="command">Команда с ID формы и новыми данными.</param>
        /// <returns>NoContent (успешно).</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, [FromBody] FLHAFormCreateDto dto)
        {
            await _mediator.Send(new UpdateFLHAFormCommand { FormId = id, Dto = dto });
            return NoContent();
        }
    }
}
