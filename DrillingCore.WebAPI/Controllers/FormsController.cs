using DrillingCore.Application.Forms.Commands;
using DrillingCore.Application.Forms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/Forms")]
    public class FormsController : Controller
    {
        private readonly IMediator _mediator;

        public FormsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all forms of a specific type associated with the given project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="formTypeId">The ID of the form type (e.g., Drill Inspection = 2).</param>
        /// <returns>A list of forms matching the specified project and form type.</returns>
        /// 
        [HttpGet("project/{projectId}/type/{formTypeId}")]
        public async Task<IActionResult> GetFormsByProjectAndType(int projectId, int formTypeId)
        {
            var result = await _mediator.Send(new GetFormsByProjectAndTypeQuery { ProjectId = projectId, FormTypeId = formTypeId });
            return Ok(result);
        }

        /// <summary>
        /// Creates a new Drill Inspection form.
        /// </summary>
        /// <param name="command">The command containing inspection data: participants, checklist responses, unit number, comments, and signatures.</param>
        /// <returns>The ID of the newly created form.</returns>
        [HttpPost("drill-inspection")]
        public async Task<IActionResult> CreateDrillInspection([FromBody] CreateDrillInspectionCommand command)
        {
            var formId = await _mediator.Send(command);
            return Ok(new { formId });
        }


        /// <summary>
        /// Gets the latest active equipment assigned to a participant for a specific form.
        /// </summary>
        /// <param name="formTypeId">The form type ID (e.g., Drill Inspection = 2).</param>
        /// <param name="participantId">The participant ID.</param>
        /// <param name="projectId">The project ID.</param>
        /// <returns>Returns the most recent active equipment of the matching type.</returns>
        /// 
        [HttpGet("equipment")]
        public async Task<IActionResult> GetEquipmentForForm([FromQuery] GetEquipmentForFormQuery query)
        {
            var result = await _mediator.Send(query);
            return result == null
                ? NotFound("No equipment found.")
                : Ok(result);
        }


        
        /// <summary>
        /// Returns all photos attached to the specified form.
        /// </summary>
        /// <param name="formId">The ID of the form.</param>
        /// <returns>A list of form photos.</returns>
        [HttpGet("{formId}/photos")]
        public async Task<IActionResult> GetFormPhotos(int formId)
        {
            var result = await _mediator.Send(new GetFormPhotosQuery { FormId = formId });
            return Ok(result);
        }

        /// <summary>
        /// Returns all participant signatures attached to the specified form.
        /// </summary>
        /// <param name="formId">The ID of the form.</param>
        /// <returns>A list of participant signatures.</returns>
        [HttpGet("{formId}/signatures")]
        public async Task<IActionResult> GetFormSignatures(int formId)
        {
            var result = await _mediator.Send(new GetFormSignaturesQuery { FormId = formId});
            return Ok(result);
        }
    }
}
