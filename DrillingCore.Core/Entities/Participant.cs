using System;

namespace DrillingCore.Core.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        // Если участник добавлен в группу – здесь хранится Id группы; если нет – NULL
        public int? GroupId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? EndDate { get; set; }

        // Навигационное свойство – связь с таблицей пользователей
        public virtual User User { get; set; }
    }
}
