using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Queries
{
    public class GetFormDeliveryRulesQuery : IRequest<List<FormDeliveryRuleDto>>
    {
        public int ProjectId { get; set; }
    }
}
