using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UpdateDrillInspectionCommandHandler : IRequestHandler<UpdateDrillInspectionCommand, Unit>
    {
        private readonly IFormRepository _formRepository;

        public UpdateDrillInspectionCommandHandler(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<Unit> Handle(UpdateDrillInspectionCommand request, CancellationToken cancellationToken)
        {
            await _formRepository.UpdateDrillInspectionAsync(request, cancellationToken);
            return Unit.Value;
        }
    }
}
