using DrillingCore.Application.Interfaces;

namespace DrillingCore.Infrastructure.Service
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(List<string> recipients, string subject, string body)
        {
            // Эмуляция отправки (можно логировать в файл или выводить в консоль)
            Console.WriteLine("=== FAKE EMAIL SENDER ===");
            Console.WriteLine($"To: {string.Join(", ", recipients)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            Console.WriteLine("==========================");
            return Task.CompletedTask;
        }

        public Task SendEmailWithAttachmentAsync(List<string> recipients, string subject, string body, byte[] attachment, string filename)
        {
            throw new NotImplementedException();
        }
    }
}
