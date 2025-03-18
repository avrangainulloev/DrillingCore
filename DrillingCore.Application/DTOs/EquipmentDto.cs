using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TypeId { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        // Дополнительно: можно вернуть название типа, если нужно
        public string? TypeName { get; set; }
    }
}
