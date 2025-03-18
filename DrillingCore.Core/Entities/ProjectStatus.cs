using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class ProjectStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Например: "Active", "Inactive", "Suspended", "Completed"
        public string Description { get; set; } = null!; // Дополнительное описание, если нужно

        // Если нужно, можно добавить коллекцию проектов, связанных с этим статусом
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
