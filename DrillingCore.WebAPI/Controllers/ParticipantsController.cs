using DrillingCore.Application.DTOs;
using DrillingCore.Application.Projects.Participants.Commands;
using DrillingCore.Application.Projects.Participants.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Projects/{projectId}/Participants")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Projects/{projectId}/Participants
        //[HttpGet]
        //public async Task<IActionResult> GetParticipants(int projectId)
        //{
        //    var query = new GetParticipantsQuery(projectId);
        //    var participants = await _mediator.Send(query);
        //    return Ok(participants);
        //}

        // POST: api/Projects/{projectId}/Participants
        [HttpPost]
        public async Task<IActionResult> AddParticipant(int projectId, [FromBody] AddParticipantCommand command)
        {
            if (projectId != command.ProjectId)
                return BadRequest("Project ID mismatch");
            int newId = await _mediator.Send(command);
            return NoContent();
        }

        // PUT: api/Projects/{projectId}/Participants/{participantId}/Finish
        [HttpPut("{participantId}/Finish")]
        public async Task<IActionResult> FinishParticipant(int projectId, int participantId, [FromBody] FinishParticipantCommand command)
        {
            if (projectId != command.ProjectId || participantId != command.ParticipantId)
                return BadRequest("Project ID or Participant ID mismatch");
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
