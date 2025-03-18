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
            int lastId = 0;
            foreach (var userId in request.UserIds)
            {
                var participant = new Participant
                {
                    ProjectId = request.ProjectId,
                    UserId = userId,
                    GroupId = request.GroupId, // если null – участник добавлен напрямую в проект
                    StartDate = request.StartDate,
                    DailyRate = request.DailyRate,
                    MeterRate = request.MeterRate,
                    DateAdded = DateTime.UtcNow

                };

                await _participantRepository.AddAsync(participant);
                lastId = participant.Id; // можно возвращать последний созданный Id или, например, количество добавленных записей
            }
            return lastId;
        }
    }
}
