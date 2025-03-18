using MediatR;

namespace DrillingCore.Application.ParticipantsEquipment.Commands
{
    public class AttachEquipmentCommand : IRequest<int>
    {
        public int ParticipantId { get; set; }
        public int EquipmentId { get; set; }    
        public DateTime StartDate { get; set; }
    }
}
