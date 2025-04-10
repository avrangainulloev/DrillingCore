using DrillingCore.Application.DTOs;
using DrillingCore.Application.Forms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [Controller]
    [Route("api/Checklist")]
    public class ChecklistItemsController : Controller
    {
        private readonly IMediator _mediator;

        public ChecklistItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns checklist items associated with the specified form type.
        /// </summary>
        /// <param name="formTypeId">The ID of the form type.</param>
        /// <returns>A list of checklist items grouped by category.</returns>
        [HttpGet("by-form-type/{formTypeId}")]
        public async Task<ActionResult<List<ChecklistItemDto>>> GetByFormType(int formTypeId)
        {
            var result = await _mediator.Send(new GetChecklistItemsQuery(formTypeId));
            return Ok(result);
            
        }
    }
}
