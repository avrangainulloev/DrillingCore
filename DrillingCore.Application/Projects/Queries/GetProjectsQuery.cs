using DrillingCore.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace DrillingCore.Application.Projects.Queries
{
    public class GetProjectsQuery : IRequest<IEnumerable<ProjectDto>>
    {
        // Можно передавать параметры запроса, например, количество проектов (limit)
        public int Limit { get; set; } = 30;
    }
}
