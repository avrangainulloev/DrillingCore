using DrillingCore.Application.Equipments.Commands.DeleteEquipment;
using DrillingCore.Application.Interfaces;
using MediatR;

namespace DrillingCore.Application.Equipments.Commands.DeleteEquipment
{
    public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand,Unit>
    {
        private readonly IEquipmentRepository _repository;

        public DeleteEquipmentCommandHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByIdAsync(request.Id);
            if (equipment == null)
            {
                throw new Exception($"Equipment with Id={request.Id} not found.");
            }

            await _repository.DeleteAsync(equipment);
            return Unit.Value;
        }
    }
}
