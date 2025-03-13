using MediatR;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    public class AddParticipantsCommand : IRequest<List<int>>
    {
        public int ProjectId { get; set; }
        public int GroupId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
