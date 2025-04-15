using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class UpdateFormDeliveryRuleHandler : IRequestHandler<UpdateFormDeliveryRuleCommand,Unit>
    {
        private readonly IFormDeliveryRepository _repository;

        public UpdateFormDeliveryRuleHandler(IFormDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateFormDeliveryRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _repository.GetRuleByIdAsync(request.Id, cancellationToken);
            if (rule == null) throw new Exception("Правило не найдено");

            rule.FormTypeId = request.FormTypeId;
            rule.ProjectId = request.ProjectId;
            rule.Condition = request.Condition;

            // Удаляем старых получателей
            rule.Recipients.Clear();

            // Добавляем новых
            foreach (var recipient in request.Recipients)
            {
                rule.Recipients.Add(new Core.Entities.FormDeliveryRecipient
                {
                    FullName = recipient.FullName,
                    Company = recipient.Company,
                    Email = recipient.Email
                });
            }

            await _repository.UpdateRuleAsync(rule, cancellationToken);
            return Unit.Value;
        }
    }
}
