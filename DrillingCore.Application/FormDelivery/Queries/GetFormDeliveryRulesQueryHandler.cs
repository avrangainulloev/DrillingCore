using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Queries
{
    public class GetFormDeliveryRulesQueryHandler : IRequestHandler<GetFormDeliveryRulesQuery, List<FormDeliveryRuleDto>>
    {
        private readonly IFormDeliveryRepository _repository;

        public GetFormDeliveryRulesQueryHandler(IFormDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FormDeliveryRuleDto>> Handle(GetFormDeliveryRulesQuery request, CancellationToken cancellationToken)
        {
            var rules = await _repository.GetRulesByProjectIdAsync(request.ProjectId, cancellationToken);

            return rules.Select(r => new FormDeliveryRuleDto
            {
                Id = r.Id,
                FormTypeId = r.FormTypeId,
                FormTypeName = r.FormType.Name,
                Condition = r.Condition.ToString(),
                Recipients = r.Recipients.Select(x => new FormDeliveryRecipientDto
                {
                    FullName = x.FullName,
                    Company = x.Company,
                    Email = x.Email
                }).ToList()
            }).ToList();
        }
    }
}
