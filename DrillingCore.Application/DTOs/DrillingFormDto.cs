using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class DrillingFormDto
    {
        public int Id { get; set; }
        public DateOnly DateFilled { get; set; }
        public string? CrewName { get; set; }
        public int ProjectId { get; set; }
        public int NumberOfWells { get; set; }
        public double TotalMeters { get; set; }
        public List<int> ParticipantIds { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new();
        public List<FormSignatureDto> Signatures { get; set; } = new();
    }
}
