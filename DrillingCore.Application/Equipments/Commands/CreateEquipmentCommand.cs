using MediatR;

namespace DrillingCore.Application.Equipments.Commands
{
    public class CreateEquipmentCommand : IRequest<int> // возвращает Id созданного оборудования
    {
        public string Name { get; set; } = null!;
        public int TypeId { get; set; }
        public string RegistrationNumber { get; set; } = null!;
    }
}
