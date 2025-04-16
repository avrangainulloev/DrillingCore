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
    public class GetFormTypesQueryHandler : IRequestHandler<GetFormTypesQuery, List<FormTypeDto>>
    {
        private readonly IFormRepository _repository;

        public GetFormTypesQueryHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FormTypeDto>> Handle(GetFormTypesQuery request, CancellationToken cancellationToken)
        {
            var formTypes = await _repository.GetFormTypesAsync();
            return formTypes.Select(f => new FormTypeDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description
            }).ToList();
        }
    }
}
