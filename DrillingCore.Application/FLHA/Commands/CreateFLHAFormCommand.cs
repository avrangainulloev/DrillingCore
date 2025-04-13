using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.FLHA.Commands
{
    public class CreateFLHAFormCommand : IRequest<int>
    {
        public FLHAFormCreateDto Dto { get; set; }       
    }
}
