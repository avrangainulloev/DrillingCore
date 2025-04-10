using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.ParticipantsEquipment.Queries
{
    public class GetAllParticipantEquipmentQueryHandler : IRequestHandler<GetAllParticipantEquipmentQuery, List<ParticipantEquipmentDto>>
    {
        private readonly IParticipantEquipmentRepository _repository;

        public GetAllParticipantEquipmentQueryHandler(IParticipantEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ParticipantEquipmentDto>> Handle(GetAllParticipantEquipmentQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetEquipmentByParticipantIdAsync(request.ParticipantId,request.ProjectId, cancellationToken);
            return assignments.Select(a => new ParticipantEquipmentDto
            {
                Id = a.Id,
                ParticipantId = a.ParticipantId,
                EquipmentId = a.EquipmentId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                ParticipantName = a.Participant?.User.FullName,
                EquipmentName = a.Equipment?.Name,
                EquipmentTypeName = a.Equipment.EquipmentType.TypeName,
                RegistrationNumber = a.Equipment.RegistrationNumber
                
            }).ToList();
        }
    }
}
