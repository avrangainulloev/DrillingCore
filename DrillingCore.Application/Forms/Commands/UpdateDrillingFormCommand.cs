using DrillingCore.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UpdateDrillingFormCommand : IRequest<Unit>
    {
        public int FormId { get; set; }
        public DateOnly DateFilled { get; set; }
        public string OtherComments { get; set; } = string.Empty;
        public int TotalWells { get; set; }
        public double TotalMeters { get; set; }
        public List<FormParticipantDto> Participants { get; set; } = new();
    }
}
