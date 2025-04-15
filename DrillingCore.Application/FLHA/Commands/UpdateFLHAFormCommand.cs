using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.FLHA.Commands
{
    public class UpdateFLHAFormCommand : IRequest<Unit>
    {
        public int FormId { get; set; }
        public FLHAFormCreateDto Dto { get; set; }
    }

}
