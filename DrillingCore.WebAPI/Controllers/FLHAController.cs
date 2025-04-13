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
    }
}
