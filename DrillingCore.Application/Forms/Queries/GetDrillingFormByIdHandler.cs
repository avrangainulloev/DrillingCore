using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetDrillingFormByIdHandler : IRequestHandler<GetDrillingFormByIdQuery, DrillingFormFullDto>
    {
        private readonly IFormRepository _repository;

        public GetDrillingFormByIdHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<DrillingFormFullDto> Handle(GetDrillingFormByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetDrillingFormByIdAsync(request.FormId, cancellationToken);
        }
    }
}
