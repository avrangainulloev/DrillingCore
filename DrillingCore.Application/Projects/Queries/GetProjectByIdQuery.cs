using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Projects.Queries
{
    public class GetProjectByIdQuery : IRequest<ProjectDto?>
    {
        public int Id { get; set; }
    }
}
