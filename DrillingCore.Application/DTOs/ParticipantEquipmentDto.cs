using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class ParticipantEquipmentDto
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int EquipmentId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        // Дополнительно: можно вывести FullName участника, Name техники и т.д.
        public string? ParticipantName { get; set; }
        public string? EquipmentName { get; set; }
        public string? EquipmentTypeName { get; set; }
        public string? RegistrationNumber { get; set; }
    }
}
