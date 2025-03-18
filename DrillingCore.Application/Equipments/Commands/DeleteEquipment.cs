using MediatR;

namespace DrillingCore.Application.Equipments.Commands.DeleteEquipment
{
    public class DeleteEquipmentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
