using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UploadFormPhotoCommand : IRequest<int>
    {
        public int ProjectFormId { get; set; }
        public IFormFile Photo { get; set; } = default!;
    }
}
