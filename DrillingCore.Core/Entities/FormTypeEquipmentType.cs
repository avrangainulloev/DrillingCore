using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormTypeEquipmentType
    {
        public int FormTypeId { get; set; }
        public FormType FormType { get; set; } = null!;

        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; } = null!;
    }
}
