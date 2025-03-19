using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Queries
{
    public class GetAllEquipmentTypesQueryHandler : IRequestHandler<GetAllEquipmentTypesQuery, List<EquipmentTypeDto>>
    {
        private readonly IEquipmentTypeRepository _repository;

        public GetAllEquipmentTypesQueryHandler(IEquipmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EquipmentTypeDto>> Handle(GetAllEquipmentTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _repository.GetAllAsync();
            return types.Select(t => new EquipmentTypeDto
            {
                Id = t.Id,
                TypeName = t.TypeName
            }).ToList();
        }
    }
}
