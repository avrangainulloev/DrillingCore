using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FLHA.Queries
{
    public class GetFLHAFormsByProjectIdQuery : IRequest<List<FLHAFormListDto>>
    {
        public int ProjectId { get; set; }
    }
}
