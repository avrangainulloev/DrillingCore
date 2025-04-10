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
    public class GetChecklistItemsHandler : IRequestHandler<GetChecklistItemsQuery, List<ChecklistItemDto>>
    {
        private readonly IChecklistRepository _repository;

        public GetChecklistItemsHandler(IChecklistRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ChecklistItemDto>> Handle(GetChecklistItemsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByFormTypeAsync(request.FormTypeId);
        }
    }
}
