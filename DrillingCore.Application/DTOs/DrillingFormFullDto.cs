using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class DrillingFormFullDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string CrewName { get; set; }
        public DateOnly DateFilled { get; set; }
        public int TotalWells { get; set; }
        public double TotalMeters { get; set; }
        public string OtherComments { get; set; } = string.Empty;
        public List<FormParticipantDto> Participants { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new();
        public List<FormSignatureDto> Signatures { get; set; } = new();
    }
}
