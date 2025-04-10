using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.ParticipantsEquipment.Queries
{
    public class GetAllParticipantEquipmentQuery : IRequest<List<ParticipantEquipmentDto>>
    {
        public int ParticipantId { get; set; }
        public int ProjectId { get; set; }
    }
}
