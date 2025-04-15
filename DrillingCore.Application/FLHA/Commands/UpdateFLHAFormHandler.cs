using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FLHA.Commands
{
    public class UpdateFLHAFormHandler : IRequestHandler<UpdateFLHAFormCommand, Unit>
    {
        private readonly IFlhaRepository _repository;

        public UpdateFLHAFormHandler(IFlhaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateFLHAFormCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateFLHAFormAsync(request.FormId, request.Dto, cancellationToken);
            return Unit.Value;
        }
    }

}
