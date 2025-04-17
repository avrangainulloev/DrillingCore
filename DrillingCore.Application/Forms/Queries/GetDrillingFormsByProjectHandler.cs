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
    public class GetDrillingFormsByProjectHandler : IRequestHandler<GetDrillingFormsByProjectQuery, List<DrillingFormListDto>>
    {
        private readonly IFormRepository _repository;

        public GetDrillingFormsByProjectHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DrillingFormListDto>> Handle(GetDrillingFormsByProjectQuery request, CancellationToken cancellationToken)
        {
            var forms = await _repository.GetDrillingFormsByProjectAsync(request.ProjectId, cancellationToken);

            return forms.Select(f => new DrillingFormListDto
            {
                Id = f.ProjectFormId,
                DateFilled = f.ProjectForm.UpdateAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                TotalWells = f.NumberOfWells,
                TotalMeters = f.TotalMeters,
                OtherComments = f.ProjectForm.OtherComments,
                CreatorName = f.ProjectForm.Creator.FullName,
                Status= f.ProjectForm.Status                
            }).ToList();
        }
    }

}
