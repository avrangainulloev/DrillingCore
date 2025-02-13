using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    public class AddParticipantCommandHandler : IRequestHandler<AddParticipantCommand, int>
    {
        private readonly IParticipantRepository _participantRepository;

        public AddParticipantCommandHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task<int> Handle(AddParticipantCommand request, CancellationToken cancellationToken)
        {
            var participant = new Participant
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                GroupId = request.GroupId, // если null – участник добавлен напрямую в проект
                DateAdded = request.DateAdded
            };

            await _participantRepository.AddAsync(participant);
            return participant.Id;
        }
    }
}
