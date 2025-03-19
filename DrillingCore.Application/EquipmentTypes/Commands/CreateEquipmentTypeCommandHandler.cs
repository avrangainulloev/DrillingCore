using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Commands
{
    public class CreateEquipmentTypeCommandHandler : IRequestHandler<CreateEquipmentTypeCommand, int>
    {
        private readonly IEquipmentTypeRepository _repository;

        public CreateEquipmentTypeCommandHandler(IEquipmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateEquipmentTypeCommand request, CancellationToken cancellationToken)
        {
            var equipmentType = new EquipmentType
            {
                TypeName = request.TypeName
            };

            await _repository.AddAsync(equipmentType);
            return equipmentType.Id;
        }
    }
}
