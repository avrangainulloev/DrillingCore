using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetFormsByProjectAndTypeQuery : IRequest<List<FormDto>>
    {
        public int ProjectId { get; set; }
        public int FormTypeId { get; set; }

    }
}
