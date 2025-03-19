using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Commands
{
    public class UpdateEquipmentTypeCommandHandler : IRequestHandler<UpdateEquipmentTypeCommand,Unit>
    {
        private readonly IEquipmentTypeRepository _repository;

        public UpdateEquipmentTypeCommandHandler(IEquipmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateEquipmentTypeCommand request, CancellationToken cancellationToken)
        {
            var equipmentType = await _repository.GetByIdAsync(request.Id);
            if (equipmentType == null)
            {
                throw new Exception($"EquipmentType with Id={request.Id} not found.");
            }

            equipmentType.TypeName = request.TypeName;
            await _repository.UpdateAsync(equipmentType);

            return Unit.Value;
        }
    }
}
