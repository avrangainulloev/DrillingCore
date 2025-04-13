using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FLHAHazardResponseDto
    {
        public int? HazardId { get; set; } // null → если это custom hazard
        public string Label { get; set; } = string.Empty; // обязательно для custom
        public string ControlResponse { get; set; } = string.Empty;
    }

    public class FLHAParticipantDto
    {
        public int ParticipantId { get; set; }
    }

}
