using MediatR;
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DrillingCore.Application.Groups.Queries
{
    public class GetEmptyGroupsQueryHandler : IRequestHandler<GetEmptyGroupsQuery, List<ProjectGroupDto>>
    {
        private readonly IProjectGroupRepository _projectGroupRepository;

        public GetEmptyGroupsQueryHandler(IProjectGroupRepository projectGroupRepository)
        {
            _projectGroupRepository = projectGroupRepository;
        }

        public async Task<List<ProjectGroupDto>> Handle(GetEmptyGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _projectGroupRepository.GetEmptyGroupsByProjectIdAsync(request.ProjectId);
            // Маппинг в DTO – здесь можно расширить по необходимости
            var groupDtos = groups.Select(g => new ProjectGroupDto
            {
                Id = g.Id,
                ProjectId = g.ProjectId,
                GroupName = g.GroupName,
                Participants = new List<ParticipantDto>() // Поскольку группа пустая, участников нет
            }).ToList();

            return groupDtos;
        }
    }
}
