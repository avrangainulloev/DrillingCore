using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class ChecklistItem
    {
        public int Id { get; set; }

        public int FormTypeId { get; set; }
        public string Label { get; set; } = default!;
        public string? GroupName { get; set; }

        public FormType FormType { get; set; } = default!;
        public ICollection<FormChecklistResponse> Responses { get; set; } = new List<FormChecklistResponse>();
    }

}
