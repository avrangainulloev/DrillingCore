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
    public class GetFLHAFormByIdHandler : IRequestHandler<GetFLHAFormByIdQuery, FLHAFormDto>
    {
        private readonly IFlhaRepository _repository;

        public GetFLHAFormByIdHandler(IFlhaRepository repository)
        {
            _repository = repository;
        }

        public async Task<FLHAFormDto> Handle(GetFLHAFormByIdQuery request, CancellationToken cancellationToken)
        {
            var form = await _repository.GetFLHAFormByIdAsync(request.FormId, cancellationToken);

            var projectForm = form.ProjectForm;

            return new FLHAFormDto
            {
                Id = form.ProjectFormId,
                DateFilled = projectForm.DateFilled,
                TaskDescription = form.TaskDescription,
                OtherComments = projectForm.OtherComments,
                ProjectId = projectForm.ProjectId,
                CreatorId = projectForm.CreatorId,
                CreatorName = projectForm.Creator.FullName,
                Status = projectForm.Status,
                Hazards = form.Hazards.Select(h => new FLHAHazardEntryDto
                {
                    HazardText = h.HazardText,
                    ControlMeasures = h.ControlMeasures,
                    HazardTemplateId = h.HazardTemplateId
                }).ToList(),
                Participants = projectForm.FormParticipants.Select(p => new ParticipantWithSignatureDto
                {
                    ParticipantId = p.ParticipantId,
                    SignatureUrl = projectForm.FormSignatures.FirstOrDefault(s => s.ParticipantId == p.ParticipantId)?.SignatureUrl
                }).ToList(),
                PhotoUrls = projectForm.FormPhotos.Select(p => p.PhotoUrl).ToList()
            };
        }
    }

}
