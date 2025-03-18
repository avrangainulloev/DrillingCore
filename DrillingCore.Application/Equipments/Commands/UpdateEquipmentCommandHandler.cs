using DrillingCore.Application.Equipments.Commands.UpdateEquipment;
using DrillingCore.Application.Interfaces;
using MediatR;

public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, Unit>
{
    private readonly IEquipmentRepository _repository;

    public UpdateEquipmentCommandHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetByIdAsync(request.Id);
        if (equipment == null)
        {
            throw new Exception($"Equipment with Id={request.Id} not found.");
        }

        equipment.Name = request.Name;
        equipment.TypeId = request.TypeId;
        equipment.RegistrationNumber = request.RegistrationNumber;

        await _repository.UpdateAsync(equipment);
        return Unit.Value;
    }
}
