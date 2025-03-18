using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class EquipmentType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = null!;

        // Пример: может хранить коллекцию Equipment
        public virtual ICollection<Equipment>? Equipments { get; set; }
    }
}
