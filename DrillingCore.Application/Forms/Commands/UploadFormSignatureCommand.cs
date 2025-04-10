using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UploadFormSignatureCommand : IRequest<int>
    {
        public int ProjectFormId { get; set; }
        public int ParticipantId { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}
