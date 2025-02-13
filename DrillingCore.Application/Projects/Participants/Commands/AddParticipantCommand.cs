using MediatR;
using System;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    // Команда возвращает Id созданного участника
    public class AddParticipantCommand : IRequest<int>
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        // Если участник добавляется в группу, здесь указывается GroupId; иначе – null
        public int? GroupId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
