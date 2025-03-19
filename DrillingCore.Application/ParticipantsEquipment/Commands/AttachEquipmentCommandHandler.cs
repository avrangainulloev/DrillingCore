using DrillingCore.Application.Interfaces;
using DrillingCore.Domain.Entities;
using MediatR;

namespace DrillingCore.Application.ParticipantsEquipment.Commands
{
    public class AttachEquipmentCommandHandler : IRequestHandler<AttachEquipmentCommand, int>
    {
        private readonly IParticipantEquipmentRepository _repository;

        public AttachEquipmentCommandHandler(IParticipantEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(AttachEquipmentCommand request, CancellationToken cancellationToken)
        {
            // Создаем запись ParticipantEquipment
            var assignment = new ParticipantEquipment
            {
                ProjectId = request.ProjectId,
                ParticipantId = request.ParticipantId,
                EquipmentId = request.EquipmentId,
                StartDate = request.StartDate
            };

            await _repository.AddAsync(assignment);
            return assignment.Id;
        }
    }
}
