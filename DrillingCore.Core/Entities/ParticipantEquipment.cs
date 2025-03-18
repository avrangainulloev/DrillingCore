using DrillingCore.Core.Entities;

namespace DrillingCore.Domain.Entities
{
    public class ParticipantEquipment
    {
        public int Id { get; set; }

        // Внешний ключ на Participant
        public int ParticipantId { get; set; }

        // Внешний ключ на Equipment
        public int EquipmentId { get; set; }

        // Когда участник начал использовать технику
        public DateTime StartDate { get; set; }

        // Когда участник закончил использовать технику (может быть null)
        public DateTime? EndDate { get; set; }

        // Навигационные свойства (не обязательны, но часто удобны)
        public virtual Participant? Participant { get; set; }
        public virtual Equipment? Equipment { get; set; }
    }
}
