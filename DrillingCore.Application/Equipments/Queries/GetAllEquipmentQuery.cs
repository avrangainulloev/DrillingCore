using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.Equipments.Queries.GetAllEquipment
{
    public class GetAllEquipmentQuery : IRequest<List<EquipmentDto>>
    {
    }
}
