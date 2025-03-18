using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.Equipments.Queries.GetEquipmentById
{
    public class GetEquipmentByIdQuery : IRequest<EquipmentDto?>
    {
        public int Id { get; set; }
    }
}
