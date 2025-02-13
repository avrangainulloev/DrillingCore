using System;

namespace DrillingCore.Application.DTOs
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        // Если участник в группе, GroupId содержит значение; иначе – null
        public int? GroupId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? EndDate { get; set; }

        // Дополнительно можно добавить поля для отображения имени и телефона пользователя,
        // которые можно получить через IUserRepository, если требуется.
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
