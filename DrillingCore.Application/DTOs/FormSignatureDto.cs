using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FormSignatureDto
    {
        public int ParticipantId { get; set; }
        public string SignatureUrl { get; set; } = default!;
    }
}
