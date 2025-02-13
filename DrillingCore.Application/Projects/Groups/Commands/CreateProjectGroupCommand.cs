using MediatR;

namespace DrillingCore.Application.Projects.Groups.Commands
{
    // Команда возвращает Id созданной группы
    public class CreateProjectGroupCommand : IRequest<int>
    {
        public int ProjectId { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }
}
