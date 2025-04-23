using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Queries.Handlers
{
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllAsync(request.Limit, request.UserId, request.SearchTerm, request.Status);

            // Маппинг сущностей Project в ProjectDto. В данном случае Status маппится через p.Status.Name.
            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Client = p.Client,
                HasCampOrHotel = p.HasCampOrHotel,
                Status = p.Status.Name,
                 StatusId = p.StatusId

            });
        }
    }
}
