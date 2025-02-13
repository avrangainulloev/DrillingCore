using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Participants.Queries
{
    public class GetProjectGroupsQueryHandler : IRequestHandler<GetProjectGroupsQuery, IEnumerable<ProjectGroupDto>>
    {
        private readonly IProjectGroupRepository _groupRepository;
        // Если нужно получить данные о пользователях, можно использовать IUserRepository:
        // private readonly IUserRepository _userRepository;

        public GetProjectGroupsQueryHandler(IProjectGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<IEnumerable<ProjectGroupDto>> Handle(GetProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _groupRepository.GetByProjectIdAsync(request.ProjectId);
            var groupDtos = groups.Select(g => new ProjectGroupDto
            {
                Id = g.Id,
                ProjectId = g.ProjectId,
                GroupName = g.GroupName,
                Participants = g.Participants.Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    ProjectId = p.ProjectId,
                    UserId = p.UserId,
                    GroupId = p.GroupId,
                    DateAdded = p.DateAdded,
                    EndDate = p.EndDate,
                    FullName = p.User?.FullName,
                    Mobile = p.User?.Mobile,
                    Email = p.User?.Email,
                    Role = p.User?.Role
                }).ToList()
            });
            return groupDtos;
        }
    }
}
