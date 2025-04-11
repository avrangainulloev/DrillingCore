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
    public class GetDrillInspectionByIdQueryHandler : IRequestHandler<GetDrillInspectionByIdQuery, DrillInspectionDto>
    {
        private readonly IFormRepository _formRepository;

        public GetDrillInspectionByIdQueryHandler(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<DrillInspectionDto> Handle(GetDrillInspectionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _formRepository.GetDrillInspectionByIdAsync(request.FormId, cancellationToken);
        }
    }
}
