using DrillingCore.Application.DTOs;
using DrillingCore.Application.FormDelivery.Commands;
using DrillingCore.Application.FormDelivery.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormDeliveryRulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FormDeliveryRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Создает новое правило отправки форм для проекта.
        /// </summary>
        /// <param name="command">Данные правила (тип формы, условие, получатели).</param>
        /// <returns>Идентификатор созданного правила.</returns>
        /// <response code="200">Правило успешно создано.</response>
        /// <response code="400">Ошибочные данные запроса.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
         
        public async Task<IActionResult> Create(CreateFormDeliveryRuleCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { RuleId = id });
        }


        /// <summary>
        /// Получает список правил отправки форм для проекта.
        /// </summary>
        /// <param name="projectId">ID проекта.</param>
        /// <returns>Список правил с получателями.</returns>
        /// <response code="200">Список правил успешно получен.</response>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(List<FormDeliveryRuleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRules(int projectId)
        {
            var result = await _mediator.Send(new GetFormDeliveryRulesQuery { ProjectId = projectId });
            return Ok(result);
        }

        /// <summary>
        /// Обновляет существующее правило отправки форм.
        /// </summary>
        /// <param name="id">ID правила.</param>
        /// <param name="command">Обновленные данные правила.</param>
        /// <returns>Результат выполнения.</returns>
        /// <response code="204">Успешно обновлено.</response>
        /// <response code="404">Правило не найдено.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFormDeliveryRuleCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID в URL и теле запроса не совпадают.");

            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Удаляет правило отправки форм.
        /// </summary>
        /// <param name="id">ID правила.</param>
        /// <returns>Результат выполнения.</returns>
        /// <response code="204">Успешно удалено.</response>
        /// <response code="404">Правило не найдено.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteFormDeliveryRuleCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Ручная отправка всех Completed форм за выбранную дату.
        /// </summary>
        /// <param name="command">Проект, тип формы и дата.</param>
        /// <returns>Результат отправки.</returns>
        [HttpPost("send-manual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendFormsManual([FromBody] SendFormsManualCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Forms sent if rule and data matched." });
        }
    }
}
