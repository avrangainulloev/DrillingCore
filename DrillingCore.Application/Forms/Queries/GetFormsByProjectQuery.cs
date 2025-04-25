using DrillingCore.Application.Common;
using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetFormsByProjectQuery : IRequest<PaginatedList<FormListItemDto>>
    {
        public int ProjectId { get; set; }
        public int? FormTypeId { get; set; }
        public int? UserId { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
       
    }
}
