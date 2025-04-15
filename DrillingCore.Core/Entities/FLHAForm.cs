using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FLHAForm
    {
        [Key]
        public int ProjectFormId { get; set; } // Совпадает с ProjectForm.Id
        public ProjectForm ProjectForm { get; set; }

        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
     
        public string Status { get; set; }
        
        public ICollection<FLHAFormHazard> Hazards { get; set; } = new List<FLHAFormHazard>();

    }
}
