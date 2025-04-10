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
    public class GetFormSignaturesHandler : IRequestHandler<GetFormSignaturesQuery, List<FormSignatureDto>>
    {
        private readonly IFormRepository _repository;

        public GetFormSignaturesHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FormSignatureDto>> Handle(GetFormSignaturesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetFormSignaturesAsync(request.FormId);
        }
    }
}
