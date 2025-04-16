using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FormDelivery.Commands
{
    public class SendFormsManualCommandHandler : IRequestHandler<SendFormsManualCommand, Unit>
    {
        private readonly IFormDeliveryRepository _repository;
        private readonly IFormDeliveryService _formDeliveryService;

        public SendFormsManualCommandHandler(
            IFormDeliveryRepository repository,
            IFormDeliveryService formDeliveryService)
        {
            _repository = repository;
            _formDeliveryService = formDeliveryService;
        }

        public async Task<Unit> Handle(SendFormsManualCommand request, CancellationToken cancellationToken)
        {
            var rule = await _repository.GetRuleAsync(request.ProjectId, request.FormTypeId, cancellationToken);

            if (rule == null || rule.Recipients == null || !rule.Recipients.Any())
                return Unit.Value;

            await _formDeliveryService.TrySendOnAllParticipantsSigned(
                request.ProjectId,
                request.FormTypeId,
                request.DateFilled,
                rule,
                cancellationToken
            );

            return Unit.Value;
        }
    }
}
