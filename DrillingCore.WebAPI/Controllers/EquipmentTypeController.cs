using DrillingCore.Application.EquipmentTypes.Commands;
using DrillingCore.Application.EquipmentTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EquipmentTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/EquipmentType
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllEquipmentTypesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST: api/EquipmentType
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentTypeCommand command)
        {
            var newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = newId }, newId);
        }

        // PUT: api/EquipmentType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentTypeCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch.");
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/EquipmentType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteEquipmentTypeCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
