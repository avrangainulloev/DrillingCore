using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Groups.Commands
{
    public class CreateProjectGroupCommandHandler : IRequestHandler<CreateProjectGroupCommand, int>
    {
        private readonly IProjectGroupRepository _groupRepository;

        public CreateProjectGroupCommandHandler(IProjectGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<int> Handle(CreateProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new ProjectGroup
            {
                ProjectId = request.ProjectId,
                GroupName = request.GroupName
            };

            await _groupRepository.AddAsync(group);
            return group.Id;
        }
    }
}
