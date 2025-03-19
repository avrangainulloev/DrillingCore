using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;

namespace DrillingCore.Application.Equipments.Commands
{
    public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, int>
    {
        private readonly IEquipmentRepository _repository;

        public CreateEquipmentCommandHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = new Equipment
            {
                Name = request.Name,
                EquipmentTypeId = request.TypeId,
                RegistrationNumber = request.RegistrationNumber,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddAsync(equipment);
            return equipment.Id;
        }
    }
}
