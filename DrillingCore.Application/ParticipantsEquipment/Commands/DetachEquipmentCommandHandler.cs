using DrillingCore.Application.Interfaces;
using DrillingCore.Application.ParticipantsEquipments.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.ParticipantsEquipment.Commands
{
    public class DetachEquipmentCommandHandler : IRequestHandler<DetachEquipmentCommand, Unit>
    {
        private readonly IParticipantEquipmentRepository _repository;

        public DetachEquipmentCommandHandler(IParticipantEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DetachEquipmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _repository.GetByIdAsync(request.Id);
            if (assignment == null)
            {
                throw new Exception($"ParticipantEquipment with Id={request.Id} not found.");
            }

            // Ставим EndDate (завершение использования)
            assignment.EndDate = request.EndDate;

            await _repository.UpdateAsync(assignment);
            return Unit.Value;
        }
    }
}
