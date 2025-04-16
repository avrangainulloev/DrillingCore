using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UpdateDrillingFormCommandHandler : IRequestHandler<UpdateDrillingFormCommand, Unit>
    {
        private readonly IFormRepository _repository;
     

        public UpdateDrillingFormCommandHandler(IFormRepository repository)
        {
            _repository = repository;
          
        }

        public async Task<Unit> Handle(UpdateDrillingFormCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDrillingFormAsync(request, cancellationToken);
            return Unit.Value;
        }
    }
}
