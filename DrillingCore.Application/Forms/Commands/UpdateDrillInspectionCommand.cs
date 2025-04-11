using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.Forms.Commands
{
    public class UpdateDrillInspectionCommand : IRequest<Unit>
    {
        public int FormId { get; set; }
        public string CrewName { get; set; } = default!;
        public string UnitNumber { get; set; } = default!;
        public DateTime DateFilled { get; set; }
        public string? OtherComments { get; set; }

        public List<ChecklistResponseDto> ChecklistResponses { get; set; } = new();
        public List<FormParticipantDto> Participants { get; set; } = new();
    }
}
