using DrillingCore.Application.Common;
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
    public class GetFormsByProjectQueryHandler
     : IRequestHandler<GetFormsByProjectQuery, PaginatedList<FormListItemDto>>
    {
        private readonly IFormRepository _formRepository;

        public GetFormsByProjectQueryHandler(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        public async Task<PaginatedList<FormListItemDto>> Handle(GetFormsByProjectQuery request, CancellationToken cancellationToken)
        {
            return await _formRepository.GetFormsByProjectAsync(
                request.ProjectId,
                request.FormTypeId,
                request.UserId,
                request.Page,
                request.Limit
            );
        }
    }
}
