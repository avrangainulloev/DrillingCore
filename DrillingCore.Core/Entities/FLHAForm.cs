using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FLHAForm
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int CreatorId { get; set; }
        public int FormTypeId { get; set; }
        public DateTime DateFilled { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
        public string OtherComments { get; set; }
        public string Status { get; set; }
        public ICollection<FLHAFormHazard> Hazards { get; set; } = new List<FLHAFormHazard>();
        public ICollection<FormParticipant> Participants { get; set; } = new List<FormParticipant>();
        public ICollection<FormPhoto> FormPhotos { get; set; } = new List<FormPhoto>();
        public ICollection<FormSignature> FormSignatures { get; set; } = new List<FormSignature>();
    }
}
