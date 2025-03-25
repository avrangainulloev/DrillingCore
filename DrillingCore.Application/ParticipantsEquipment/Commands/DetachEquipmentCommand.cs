using MediatR;

namespace DrillingCore.Application.ParticipantsEquipments.Commands
{
    public class DetachEquipmentCommand : IRequest<Unit>
    {
        public int Id { get; set; } // Id записи ParticipantEquipment
        public DateOnly EndDate { get; set; }
    }
}
