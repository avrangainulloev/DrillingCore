using MediatR;
using DrillingCore.Application.DTOs;
using System.Collections.Generic;

namespace DrillingCore.Application.Groups.Queries
{
    public class GetEmptyGroupsQuery : IRequest<List<ProjectGroupDto>>
    {
        public int ProjectId { get; set; }

        public GetEmptyGroupsQuery(int projectId)
        {
            ProjectId = projectId;
        }
    }
}
