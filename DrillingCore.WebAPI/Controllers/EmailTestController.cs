using DrillingCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DrillingCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailTestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        /// <summary>
        /// Отправить тестовое письмо без вложения
        /// </summary>
        [HttpGet("test-send")]
        public async Task<IActionResult> Send()
        {
            var recipients = new List<string> { "avrang.ainulloev@gmail.com" };
            var subject = "Test Email from DrillingCore";
            var body = "This is a test email from your system.";

            await _emailSender.SendEmailAsync(recipients, subject, body);

            return Ok("Email sent (if all configs are valid).");
        }
    }
}
