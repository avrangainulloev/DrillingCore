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
    public class GetFormsByProjectAndTypeHandler : IRequestHandler<GetFormsByProjectAndTypeQuery, List<FormDto>>
    {
        private readonly IFormRepository _repository;

        public GetFormsByProjectAndTypeHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FormDto>> Handle(GetFormsByProjectAndTypeQuery request, CancellationToken cancellationToken)
        {
            var forms = await _repository.GetFormsByProjectAndTypeAsync(request.ProjectId, request.FormTypeId);

            return forms.Select(f => new FormDto
            {
                Id = f.Id,
                FormTypeName = f.FormType.Name,
                CrewName = f.CrewName,
                UnitNumber = f.UnitNumber,
                DateFilled = f.UpdateAt.ToLocalTime(),
                OtherComments = f.OtherComments,
                Status = f.Status
            }).ToList();
        }
    }
}
