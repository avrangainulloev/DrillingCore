using DrillingCore.Application.DTOs;
using DrillingCore.Application.Equipments.Queries.GetAllEquipment;
using DrillingCore.Application.Interfaces;
using MediatR;

namespace DrillingCore.Application.Equipments.Queries.GetAllEquipment
{
    public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, List<EquipmentDto>>
    {
        private readonly IEquipmentRepository _repository;

        public GetAllEquipmentQueryHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EquipmentDto>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
        {
            var equipments = await _repository.GetAllAsync();
            return equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                TypeId = e.TypeId,
                RegistrationNumber = e.RegistrationNumber,
                CreatedDate = e.CreatedDate,
                TypeName = e.EquipmentType?.TypeName
            }).ToList();
        }
    }
}
