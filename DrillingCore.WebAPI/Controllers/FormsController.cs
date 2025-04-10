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

    }
}
