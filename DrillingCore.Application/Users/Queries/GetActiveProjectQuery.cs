using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetActiveProjectQuery : IRequest<ActiveProjectDto>
    {
        public int UserId { get; set; }

       
    }
}
