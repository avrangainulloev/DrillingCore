using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Queries
{
    public class GetAllEquipmentTypesQuery : IRequest<List<EquipmentTypeDto>>
    {
    }
}
