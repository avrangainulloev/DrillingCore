using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Participants.Commands
{
    public class FinishParticipantCommandHandler : IRequestHandler<FinishParticipantCommand, Unit>
    {
        private readonly IParticipantRepository _participantRepository;

        public FinishParticipantCommandHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task<Unit> Handle(FinishParticipantCommand request, CancellationToken cancellationToken)
        {
            var participant = await _participantRepository.GetByIdAsync(request.ParticipantId);
            if (participant == null || participant.ProjectId != request.ProjectId)
            {
                throw new Exception("Participant not found for the specified project");
            }

            participant.EndDate = request.FinishDate;
            await _participantRepository.UpdateAsync(participant);

            return Unit.Value;
        }
    }
}
