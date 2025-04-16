using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class CreateDrillingFormCommand : IRequest<int>
    {
        public int ProjectId { get; set; }
        public int CreatorId { get; set; }
        public DateOnly DateFilled { get; set; }
        public string? OtherComments { get; set; }
        public string? UnitNumber { get; set; }
        public string? CrewName { get; set; }

        public int NumberOfWells { get; set; }
        public double TotalMeters { get; set; }

        public List<FormParticipantDto> Participants { get; set; } = new();
    }
}
