using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class ProjectForm
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FormTypeId { get; set; }
        public int CreatorId { get; set; }

        public string CrewName { get; set; } = default!;
        public string UnitNumber { get; set; } = default!;
        public DateOnly DateFilled { get; set; }  // Например: 2025-04-15
        public DateTime CreatedAt { get; set; }   // Например: 2025-04-14 22:15:03
        public DateTime UpdateAt { get; set; }   // Например: 2025-04-14 22:15:03
        public string? OtherComments { get; set; }
        public string? AdditionalData { get; set; }

        public FormType FormType { get; set; } = default!;
        public Project Project { get; set; } = default!;
        public User Creator { get; set; } = default!;
        public string Status { get; set; }

        public ICollection<FormSignature> FormSignatures { get; set; } = new List<FormSignature>();
        public ICollection<FormPhoto> FormPhotos { get; set; } = new List<FormPhoto>();
        public ICollection<FormChecklistResponse> FormChecklistResponses { get; set; } = new List<FormChecklistResponse>();
        public ICollection<FormParticipant> FormParticipants { get; set; } = new List<FormParticipant>();
        public FLHAForm? FLHAForm { get; set; } // Навигация (если форма FLHA)
        public DrillingForm? DrillingForm { get; set; }


    }
}
