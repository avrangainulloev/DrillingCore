using DrillingCore.Application.ParticipantsEquipment.Commands;
using DrillingCore.Application.ParticipantsEquipment.Queries;
using DrillingCore.Application.ParticipantsEquipments.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantEquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipantEquipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/ParticipantEquipment/5
        [HttpGet("{participantId}")]
        public async Task<IActionResult> GetEquipmentForParticipant(int participantId)
        {
            var query = new GetAllParticipantEquipmentQuery { ParticipantId = participantId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST: api/ParticipantEquipment/attach
        [HttpPost("attach")]
        public async Task<IActionResult> AttachEquipment([FromBody] AttachEquipmentCommand command)
        {
            var newId = await _mediator.Send(command);
            return Created("", newId);
        }

        // PUT: api/ParticipantEquipment/detach/{id}
        // Предположим, что detach — это обновление EndDate
        [HttpPut("detach/{id}")]
        public async Task<IActionResult> DetachEquipment(int id, [FromBody] DetachEquipmentCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch.");

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
