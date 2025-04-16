using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class SendFormsManualCommand : IRequest<Unit>
    {
        public int ProjectId { get; set; }
        public int FormTypeId { get; set; }
        public DateOnly DateFilled { get; set; }
    }
}
