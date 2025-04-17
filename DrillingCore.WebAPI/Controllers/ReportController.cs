using DrillingCore.Application.DTOs;
using DrillingCore.Application.Report.Commands;
using DrillingCore.Application.Report.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Generates a Timesheet Excel report for a specific user and date range.
        /// </summary>
        /// <param name="command">The command with user ID and date range.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Excel file (.xlsx) as downloadable result.</returns>
        [HttpPost("timesheet/excel")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateTimesheet(
            [FromBody] GenerateTimesheetReportCommand command,
            CancellationToken cancellationToken)
        {
            if (command.FromDate > command.ToDate)
                return BadRequest("Invalid date range: 'FromDate' must be earlier than or equal to 'ToDate'.");

            var excelBytes = await _mediator.Send(command, cancellationToken);

            var fileName = $"Timesheet_{command.UserId}_{command.FromDate:yyyyMMdd}_{command.ToDate:yyyyMMdd}.xlsx";

            return File(
                fileContents: excelBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: fileName);
        }


        /// <summary>
        /// Получает предварительный список дней для Timesheet по выбранному пользователю и периоду.
        /// </summary>
        /// <param name="query">Объект запроса с UserId, FromDate и ToDate.</param>
        /// <param name="ct">Токен отмены для поддержки отмены запроса.</param>
        /// <returns>Список объектов TimesheetDayDto — по одному на каждый день периода.</returns>
        [HttpPost("timesheet/preview")]
        [ProducesResponseType(typeof(List<TimesheetDayDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<TimesheetDayDto>>> GetPreview([FromBody] GetTimesheetPreviewQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }
    }
}
