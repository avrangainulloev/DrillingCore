using DrillingCore.Core.Entities;

namespace DrillingCore.Domain.Entities
{
    public class ParticipantEquipment
    {
        public int Id { get; set; }

        // Внешний ключ на проект
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        // Внешний ключ на участника
        public int ParticipantId { get; set; }
        public virtual Participant? Participant { get; set; }

        // Внешний ключ на оборудование
        public int EquipmentId { get; set; }
        public virtual Equipment? Equipment { get; set; }

        // Дата начала использования
        public DateTime StartDate { get; set; }

        // Дата окончания использования (nullable)
        public DateTime? EndDate { get; set; }
    }
}
