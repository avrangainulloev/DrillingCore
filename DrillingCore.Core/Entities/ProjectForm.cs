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
        public DateTime DateFilled { get; set; }
        public string? OtherComments { get; set; }
        public string? AdditionalData { get; set; }

        public FormType FormType { get; set; } = default!;
        public Project Project { get; set; } = default!;
        public User Creator { get; set; } = default!;
    }
}
