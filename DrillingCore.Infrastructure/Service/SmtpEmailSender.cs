using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DrillingCore.Application.Interfaces;

namespace DrillingCore.Infrastructure.Service
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(List<string> recipients, string subject, string body)
        {
            await SendEmailWithAttachmentAsync(recipients, subject, body, null, null);
        }

        public async Task SendEmailWithAttachmentAsync(List<string> recipients, string subject, string body, byte[]? attachment, string? filename)
        {
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var displayName = _config["Email:SenderName"] ?? "DrillingCore System";

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage()
            {
                From = new MailAddress(smtpUser!, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            recipients.ForEach(r => mail.To.Add(r));

            if (attachment != null && filename != null)
                mail.Attachments.Add(new Attachment(new MemoryStream(attachment), filename));

            await client.SendMailAsync(mail);
        }
    }
}
