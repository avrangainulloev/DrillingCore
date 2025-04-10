using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetEquipmentForFormQuery : IRequest<EquipmentForFormDto>
    {
        public int FormTypeId { get; set; }
        public int ParticipantId { get; set; }
        public int ProjectId { get; set; }
    }
}
