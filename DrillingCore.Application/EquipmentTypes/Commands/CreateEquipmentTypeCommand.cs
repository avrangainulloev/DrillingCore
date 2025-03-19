using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Commands
{
    public class CreateEquipmentTypeCommand : IRequest<int> // возвращает Id созданного типа
    {
        public string TypeName { get; set; } = null!;
    }
}
