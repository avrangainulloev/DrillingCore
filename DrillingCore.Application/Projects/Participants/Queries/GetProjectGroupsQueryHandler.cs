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
        private readonly IProjectRepository _projectRepository;
        
        // Если нужно получить данные о пользователях, можно использовать IUserRepository:
        // private readonly IUserRepository _userRepository;

        public GetProjectGroupsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectGroupDto>> Handle(GetProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            return await _projectRepository.GetProjectGroupsWithParticipantsAsync(request.ProjectId);
        }
    }
}
