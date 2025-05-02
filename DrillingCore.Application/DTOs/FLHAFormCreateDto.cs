using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FLHAFormCreateDto
    {
        public string CrewName { get; set; }
        public int ProjectId { get; set; }
        public int FormTypeId { get; set; } = 3; // FLHA (например)
        public int CreatorId { get; set; }

        public DateOnly DateFilled { get; set; }
        public string TaskDescription { get; set; }
        public string? OtherComments { get; set; }

        public List<FLHAHazardEntryDto> Hazards { get; set; } = new();
        //public List<int> ParticipantIds { get; set; } = new();
        // 🔁 Изменено:
        public List<ParticipantWithSignatureDto> Participants { get; set; } = new();
    }

    public class FLHAHazardEntryDto
    {
        public string HazardText { get; set; } = string.Empty;
        public string ControlMeasures { get; set; } = string.Empty;
        public int? HazardTemplateId { get; set; }
    }

}
