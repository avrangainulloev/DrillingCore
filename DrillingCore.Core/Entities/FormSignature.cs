using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormSignature
    {
        public int Id { get; set; }

        public int ProjectFormId { get; set; }
        public int ParticipantId { get; set; }

        public string SignatureUrl { get; set; } = default!;
        public DateTime CreatedDate { get; set; }

        public ProjectForm ProjectForm { get; set; } = default!;
        public Participant Participant { get; set; } = null!;
    }
}
