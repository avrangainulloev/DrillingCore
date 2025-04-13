using DrillingCore.Application.DTOs;
using DrillingCore.Application.Forms.Commands;
using DrillingCore.Application.Forms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                ? Ok("No equipment found.")
                : Ok(result);
        }


   


        /// <summary>
        /// Uploads a photo for a form.
        /// </summary>
        /// <param name="projectFormId">ID of the form (ProjectForm)</param>
        /// <param name="file">Image file to upload</param>
        /// <returns>Returns the URL of the uploaded photo</returns>
        /// <response code="200">Photo uploaded successfully</response>
        /// <response code="400">If no file is provided</response>
        [HttpPost("{projectFormId}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadPhoto(int projectFormId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var command = new UploadFormPhotoCommand
            {
                ProjectFormId = projectFormId,
                Photo = file
            };

            var result = await _mediator.Send(command);
            return Ok(result); // result может быть url или объект
        }

        /// <summary>
        /// Uploads a signature image for a participant of a form.
        /// </summary>
        /// <param name="projectFormId">ID of the form (ProjectForm)</param>
        /// <param name="participantId">ID of the participant signing</param>
        /// <param name="file">Signature file</param>
        /// <returns>Returns the URL of the uploaded signature</returns>
        /// <response code="200">Signature uploaded successfully</response>
        /// <response code="400">If no file is provided</response>
        [HttpPost("{projectFormId}/signatures")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadSignature(int projectFormId, [FromForm] int participantId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No signature file uploaded.");

            var command = new UploadFormSignatureCommand
            {
                ProjectFormId = projectFormId,
                ParticipantId = participantId,
                File = file
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Returns a full drill inspection form by ID.
        /// </summary>
        /// <param name="formId">ID of the form</param>
        /// <returns>Full DrillInspectionDto</returns>
        [HttpGet("drill-inspection/{formId}")]
        [ProducesResponseType(typeof(DrillInspectionDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDrillInspectionById(int formId)
        {

            var result = await _mediator.Send(new GetDrillInspectionByIdQuery { FormId = formId });
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing Drill Inspection form.
        /// </summary>
        /// <param name="command">The updated drill inspection form data.</param>
        /// <returns>NoContent if the update is successful.</returns>
        /// <remarks>
        /// This endpoint updates the core form details, checklist responses, and participants.
        /// Photos and signatures should be uploaded separately via the designated endpoints:
        /// - POST /api/forms/{formId}/photos
        /// - POST /api/forms/{formId}/signatures
        /// </remarks>
        /// <response code="204">Form updated successfully</response>
        /// <response code="400">Invalid data or missing fields</response>
        /// <response code="404">Form not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("drill-inspection")]
        public async Task<IActionResult> UpdateDrillInspection([FromBody] UpdateDrillInspectionCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
