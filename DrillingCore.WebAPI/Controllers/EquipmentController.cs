using DrillingCore.Application.Equipments.Commands.DeleteEquipment;
using DrillingCore.Application.Equipments.Commands.UpdateEquipment;
using DrillingCore.Application.Equipments.Commands;
using DrillingCore.Application.Equipments.Queries.GetAllEquipment;
using DrillingCore.Application.Equipments.Queries.GetEquipmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DrillingCore.Application.Equipments.Queries;

namespace DrillingCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EquipmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Equipment
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] int? equipmentTypeId)
        {
            var query = new GetAllEquipmentQuery
            {
                SearchTerm = searchTerm,
                EquipmentTypeId = equipmentTypeId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/Equipment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEquipmentByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound($"Equipment with id {id} not found.");
            return Ok(result);
        }

        // POST: api/Equipment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentCommand command)
        {
            var newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        // PUT: api/Equipment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch.");
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/Equipment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteEquipmentCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("free")]
        public async Task<IActionResult> GetFreeEquipment()
        {
            var query = new GetFreeEquipmentQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
