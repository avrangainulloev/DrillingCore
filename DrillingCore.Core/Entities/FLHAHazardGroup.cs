using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FLHAHazardGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FLHAHazard> Hazards { get; set; }
    }
}
