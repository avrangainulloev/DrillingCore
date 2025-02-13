using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Commands.Handlers
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
    {
        private readonly IProjectRepository _projectRepository;

        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var dto = request.ProjectDto;
            var project = new Project
            {
                Name = dto.Name,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Client = dto.Client,
                HasCampOrHotel = dto.HasCampOrHotel
            };

            await _projectRepository.AddAsync(project);
            return project.Id;
        }
    }
}
