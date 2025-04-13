using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FLHA.Queries
{
    public class GetFlhaHazardsByGroupIdHandler : IRequestHandler<GetHazardsByGroupIdQuery, List<FLHAHazardDto>>
    {
        private readonly IFlhaRepository _flhaRepository;

        public GetFlhaHazardsByGroupIdHandler(IFlhaRepository flhaRepository)
        {
            _flhaRepository = flhaRepository;
        }

        public async Task<List<FLHAHazardDto>> Handle(GetHazardsByGroupIdQuery request, CancellationToken cancellationToken)
        {
            var hazards = await _flhaRepository.GetHazardsByGroupIdAsync(request.GroupId, cancellationToken);

            return hazards.Select(h => new FLHAHazardDto
            {
                Id = h.Id,
                Label = h.Label,
                ControlSuggestion = h.ControlSuggestion
            }).ToList();
        }
    }
}
