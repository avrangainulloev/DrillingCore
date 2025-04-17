using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public record GetDrillingFormsByProjectQuery(int ProjectId) : IRequest<List<DrillingFormListDto>>;

}
