using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Commands
{
    public class CreateProjectCommand : IRequest<int>
    {
        public ProjectDto ProjectDto { get; set; } = null!;
    }
}
