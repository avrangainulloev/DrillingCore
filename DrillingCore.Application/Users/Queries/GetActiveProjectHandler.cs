using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetActiveProjectHandler : IRequestHandler<GetActiveProjectQuery, ActiveProjectDto?>
    {
        private readonly IProjectRepository _repository;

        public GetActiveProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActiveProjectDto?> Handle(GetActiveProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetActiveProjectByUserAsync(request.UserId);
            return project == null ? null : new ActiveProjectDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name
            };
        }
    }

}
