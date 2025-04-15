using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class DeleteFormDeliveryRuleCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
