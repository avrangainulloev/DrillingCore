using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class DeleteFormDeliveryRuleHandler : IRequestHandler<DeleteFormDeliveryRuleCommand, Unit>
    {
        private readonly IFormDeliveryRepository _repository;

        public DeleteFormDeliveryRuleHandler(IFormDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteFormDeliveryRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _repository.GetRuleByIdAsync(request.Id, cancellationToken);
            if (rule == null)
                throw new Exception("Правило не найдено.");

            await _repository.DeleteRuleAsync(rule, cancellationToken);
            return Unit.Value;
        }
    }
}
