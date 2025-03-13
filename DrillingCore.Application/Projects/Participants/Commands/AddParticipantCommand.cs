using MediatR;
using System;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    // Команда возвращает Id созданного участника
    public class AddParticipantCommand : IRequest<int>
    {
        public int ProjectId { get; set; }
        public List<int> UserIds { get; set; }
        // Если участник добавляется в группу, здесь указывается GroupId; иначе – null
        public int? GroupId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Новые поля:
        public decimal? DailyRate { get; set; }
        public decimal? MeterRate { get; set; }
    }
}
