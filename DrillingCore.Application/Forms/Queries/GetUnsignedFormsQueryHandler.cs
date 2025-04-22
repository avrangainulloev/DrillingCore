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
    public class GetUnsignedFormsQueryHandler : IRequestHandler<GetUnsignedFormsQuery, List<UnsignedFormDto>>
    {
        private readonly IFormRepository _formRepository;

        public GetUnsignedFormsQueryHandler(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<List<UnsignedFormDto>> Handle(GetUnsignedFormsQuery request, CancellationToken cancellationToken)
        {
            return await _formRepository.GetUnsignedFormsAsync(request.UserId, cancellationToken);
        }
    }
}
