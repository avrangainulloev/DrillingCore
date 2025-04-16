using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class DrillingForm
    {
        public int Id { get; set; }
        public int ProjectFormId { get; set; } // внешний ключ, используется как ID
        public ProjectForm ProjectForm { get; set; } = null!;

        public int NumberOfWells { get; set; }
        public double TotalMeters { get; set; }
    }

}
