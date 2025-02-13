using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Commands.Handlers
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
    {
        private readonly IProjectRepository _projectRepository;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            ProjectDto dto = request.ProjectDto;
            // Получаем проект из базы данных по идентификатору
            var project = await _projectRepository.GetByIdAsync(dto.Id);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Обновляем поля проекта
            project.Name = dto.Name;
            project.Client = dto.Client;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.HasCampOrHotel = dto.HasCampOrHotel;
            // Если нужны дополнительные поля – обновите их здесь

            // Сохраняем изменения (в зависимости от реализации репозитория)
            await _projectRepository.UpdateAsync(project);

            return Unit.Value;
        }
    }
}
