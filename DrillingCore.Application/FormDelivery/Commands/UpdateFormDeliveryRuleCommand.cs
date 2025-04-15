using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class UpdateFormDeliveryRuleCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int FormTypeId { get; set; }
        public int ProjectId { get; set; }
        public FormDeliveryCondition Condition { get; set; }
        public List<FormDeliveryRecipientDto> Recipients { get; set; } = new();
    }
}
