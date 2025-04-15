using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(List<string> recipients, string subject, string body);
        Task SendEmailWithAttachmentAsync(List<string> recipients, string subject, string body, byte[] attachment, string filename);
    }
}
