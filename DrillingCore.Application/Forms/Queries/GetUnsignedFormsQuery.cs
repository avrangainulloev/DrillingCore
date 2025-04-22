using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetUnsignedFormsQuery : IRequest<List<UnsignedFormDto>>
    {
        public int UserId { get; set; }
    }
}
