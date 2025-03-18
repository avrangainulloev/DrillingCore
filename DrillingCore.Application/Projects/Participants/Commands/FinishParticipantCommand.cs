using MediatR;
using System;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    public class FinishParticipantCommand : IRequest<Unit>
    {
        public int ProjectId { get; set; }
        public int ParticipantId { get; set; }
        public DateOnly FinishDate { get; set; }
    }
}
