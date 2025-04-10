using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormParticipant
    {
        public int Id { get; set; }

        public int ProjectFormId { get; set; }
        public int ParticipantId { get; set; }

        public DateTime AttachDate { get; set; }
        public DateTime? DetachDate { get; set; }
        public string? Signature { get; set; }

        public ProjectForm ProjectForm { get; set; } = default!;
        public Participant Participant { get; set; } = default!;
    }

}
