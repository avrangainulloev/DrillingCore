using MediatR;

namespace DrillingCore.Application.Equipments.Commands.UpdateEquipment
{
    public class UpdateEquipmentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TypeId { get; set; }
        public string RegistrationNumber { get; set; } = null!;
    }
}
