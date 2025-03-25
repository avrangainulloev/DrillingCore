using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Equipments.Queries
{
    public class GetFreeEquipmentQueryHandler : IRequestHandler<GetFreeEquipmentQuery, List<EquipmentDto>>
    {
        private readonly IEquipmentRepository _repository;

        public GetFreeEquipmentQueryHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EquipmentDto>> Handle(GetFreeEquipmentQuery request, CancellationToken cancellationToken)
        {
            var equipments = await _repository.GetFreeEquipmentAsync(cancellationToken);
            return equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                TypeId = e.EquipmentTypeId,
                RegistrationNumber = e.RegistrationNumber,
                CreatedDate = e.CreatedDate,
                TypeName = e.EquipmentType?.TypeName
            }).ToList();
        }
    }
}
