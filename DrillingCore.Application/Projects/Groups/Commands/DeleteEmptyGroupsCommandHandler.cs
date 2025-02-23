using DrillingCore.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Groups.Commands
{
    public class DeleteEmptyGroupsCommandHandler : IRequestHandler<DeleteEmptyGroupsCommand,Unit>
    {
        private readonly IProjectGroupRepository _groupRepository;

        public DeleteEmptyGroupsCommandHandler(IProjectGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<Unit> Handle(DeleteEmptyGroupsCommand request, CancellationToken cancellationToken)
        {
            foreach (var groupName in request.Groups)
            {
                // Получаем группу по проекту и имени
                var group = await _groupRepository.GetByProjectIdAndNameAsync(request.ProjectId, groupName);
                if (group != null && (group.Participants == null || group.Participants.Count == 0))
                {
                    await _groupRepository.DeleteAsync(group);
                }
            }
            return Unit.Value;
        }
    }
}
