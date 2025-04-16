using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FLHA.Queries
{
    public class GetFLHAFormsByProjectIdHandler : IRequestHandler<GetFLHAFormsByProjectIdQuery, List<FLHAFormListDto>>
    {
        private readonly IFlhaRepository _repository;

        public GetFLHAFormsByProjectIdHandler(IFlhaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FLHAFormListDto>> Handle(GetFLHAFormsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var forms = await _repository.GetFLHAFormsByProjectIdAsync(request.ProjectId, cancellationToken);

            return forms.Select(f => new FLHAFormListDto
            {
                Id = f.ProjectFormId,
                DateFilled = f.ProjectForm.UpdateAt,
                TaskDescription = f.TaskDescription,
                OtherComments = f.ProjectForm.OtherComments,
                Status = f.ProjectForm.Status,
                CreatorName = f.ProjectForm.Creator != null
                    ? f.ProjectForm.Creator.FullName
                    : "—"
            }).ToList();
        }
    }
}
