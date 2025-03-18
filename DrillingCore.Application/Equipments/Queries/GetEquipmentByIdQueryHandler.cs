using DrillingCore.Application.DTOs;
using DrillingCore.Application.Equipments.Queries.GetEquipmentById;
using DrillingCore.Application.Interfaces;
using MediatR;

namespace DrillingCore.Application.Equipments.Queries.GetEquipmentById
{
    public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
    {
        private readonly IEquipmentRepository _repository;

        public GetEquipmentByIdQueryHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByIdAsync(request.Id);
            if (equipment == null) return null;
            return new EquipmentDto
            {
                Id = equipment.Id,
                Name = equipment.Name,
                TypeId = equipment.TypeId,
                RegistrationNumber = equipment.RegistrationNumber,
                CreatedDate = equipment.CreatedDate,
                TypeName = equipment.EquipmentType?.TypeName
            };
        }
    }
}
