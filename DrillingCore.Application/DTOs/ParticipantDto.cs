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
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public decimal? DailyRate { get; set; }
        public decimal? MeterRate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        //public DateTime DateAdded { get; set; }
    }
}
