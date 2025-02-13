using DrillingCore.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace DrillingCore.Application.Projects.Participants.Queries
{
    public class GetProjectGroupsQuery : IRequest<IEnumerable<ProjectGroupDto>>
    {
        public int ProjectId { get; }

        public GetProjectGroupsQuery(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
