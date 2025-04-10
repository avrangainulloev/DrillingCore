using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetEquipmentForFormHandler : IRequestHandler<GetEquipmentForFormQuery, EquipmentForFormDto?>
    {
        private readonly IFormRepository _formRepository;
        private readonly IParticipantEquipmentRepository _equipmentRepository;

        public GetEquipmentForFormHandler(
            IFormRepository formRepository,
            IParticipantEquipmentRepository equipmentRepository)
        {
            _formRepository = formRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<EquipmentForFormDto?> Handle(GetEquipmentForFormQuery request, CancellationToken cancellationToken)
        {
            var equipmentTypeId = await _formRepository.GetEquipmentTypeIdForFormTypeAsync(request.FormTypeId);
            if (equipmentTypeId == null)
                return null;

            return await _equipmentRepository.GetParticipantActiveEquipmentByTypeAsync(
                request.ParticipantId, request.ProjectId, equipmentTypeId.Value);
        }
    }

}
