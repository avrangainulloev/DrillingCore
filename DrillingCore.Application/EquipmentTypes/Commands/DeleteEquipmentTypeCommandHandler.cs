using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Commands
{
    public class DeleteEquipmentTypeCommandHandler : IRequestHandler<DeleteEquipmentTypeCommand,Unit>
    {
        private readonly IEquipmentTypeRepository _repository;

        public DeleteEquipmentTypeCommandHandler(IEquipmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEquipmentTypeCommand request, CancellationToken cancellationToken)
        {
            var equipmentType = await _repository.GetByIdAsync(request.Id);
            if (equipmentType == null)
            {
                throw new Exception($"EquipmentType with Id={request.Id} not found.");
            }
            await _repository.DeleteAsync(equipmentType);
            return Unit.Value;
        }
    }
}
