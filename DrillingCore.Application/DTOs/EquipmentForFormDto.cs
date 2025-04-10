using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class EquipmentForFormDto
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int EquipmentTypeId { get; set; }
    }
}
