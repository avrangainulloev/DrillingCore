using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetDrillInspectionByIdQuery : IRequest<DrillInspectionDto>
    {
        public int FormId { get; set; }
    }
}
