using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class CreateFormDeliveryRuleHandler : IRequestHandler<CreateFormDeliveryRuleCommand, int>
    {
        private readonly IFormDeliveryRepository _repository;

        public CreateFormDeliveryRuleHandler(IFormDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateFormDeliveryRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = new FormDeliveryRule
            {
                ProjectId = request.ProjectId,
                FormTypeId = request.FormTypeId,
                Condition = request.Condition,
                Recipients = request.Recipients.Select(r => new FormDeliveryRecipient
                {
                    FullName = r.FullName,
                    Company = r.Company,
                    Email = r.Email
                }).ToList()
            };

            await _repository.AddRuleAsync(rule, cancellationToken);
            return rule.Id;
        }
    }

}
