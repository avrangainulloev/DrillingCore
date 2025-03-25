using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Equipments.Queries
{
    public class GetFreeEquipmentQuery : IRequest<List<EquipmentDto>>
    {
    }
}
