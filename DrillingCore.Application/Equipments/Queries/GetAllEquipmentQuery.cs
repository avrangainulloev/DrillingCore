using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.Equipments.Queries.GetAllEquipment
{
    public class GetAllEquipmentQuery : IRequest<List<EquipmentDto>>
    {
        public string? SearchTerm { get; set; }
        public int? EquipmentTypeId { get; set; }
        public int Limit { get; set; } = 1000;
    }
}
