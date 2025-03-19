using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.EquipmentTypes.Commands
{
    public class UpdateEquipmentTypeCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = null!;
    }
}
