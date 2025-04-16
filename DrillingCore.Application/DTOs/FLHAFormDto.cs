using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FLHAFormDto
    {
        public int Id { get; set; }
        public DateOnly DateFilled { get; set; }
        public string TaskDescription { get; set; }
        public string? OtherComments { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }

        public List<FLHAHazardEntryDto> Hazards { get; set; } = new();
        public List<ParticipantWithSignatureDto> Participants { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new();
    }

    public class ParticipantWithSignatureDto
    {
        public int ParticipantId { get; set; }
        public string? SignatureUrl { get; set; }
    }

}
