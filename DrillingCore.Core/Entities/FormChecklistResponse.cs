using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormChecklistResponse
    {
        public int Id { get; set; }

        public int ProjectFormId { get; set; }
        public int ChecklistItemId { get; set; }
        public bool Response { get; set; }

        public ProjectForm ProjectForm { get; set; } = default!;
        public ChecklistItem ChecklistItem { get; set; } = default!;
    }

}
