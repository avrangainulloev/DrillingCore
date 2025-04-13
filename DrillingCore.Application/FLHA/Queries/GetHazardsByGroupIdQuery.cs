using DrillingCore.Application.DTOs;
using MediatR;

namespace DrillingCore.Application.FLHA.Queries
{
    public class GetHazardsByGroupIdQuery : IRequest<List<FLHAHazardDto>>
    {
        public int GroupId { get; set; }

    }
}
