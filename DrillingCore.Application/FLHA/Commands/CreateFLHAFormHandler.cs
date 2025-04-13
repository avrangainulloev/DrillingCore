using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.FLHA.Commands
{
    public class CreateFLHAFormHandler : IRequestHandler<CreateFLHAFormCommand, int>
    {
        private readonly IFlhaRepository _flhaRepository;
        private readonly IFormRepository _formRepository;

        public CreateFLHAFormHandler(IFlhaRepository flhaRepository, IFormRepository formRepository)
        {
            _flhaRepository = flhaRepository;
            _formRepository = formRepository;
        }

        public async Task<int> Handle(CreateFLHAFormCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var form = new FLHAForm
            {
                ProjectId = dto.ProjectId,
                CreatorId = dto.CreatorId,
                FormTypeId = dto.FormTypeId,
                DateFilled = dto.DateFilled,
                TaskDescription = dto.TaskDescription,
                OtherComments = dto.OtherComments,
                Status = "Pending",
                Hazards = dto.Hazards.Select(h => new FLHAFormHazard
                {
                    HazardText = h.HazardText,
                    ControlMeasures = h.ControlMeasures,
                    HazardTemplateId = h.HazardTemplateId
                }).ToList(),

                Participants = dto.ParticipantIds.Select(pid => new FormParticipant
                {
                    ParticipantId = pid
                }).ToList()
            };

            await _flhaRepository.AddFlhaFormAsync(form, cancellationToken);
            return form.Id;
        }
    }
}
