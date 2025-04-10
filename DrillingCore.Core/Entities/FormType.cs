using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormType
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<ProjectForm> ProjectForms { get; set; } = new List<ProjectForm>();
    }
}
