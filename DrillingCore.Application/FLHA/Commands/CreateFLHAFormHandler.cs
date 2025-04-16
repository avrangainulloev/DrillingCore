using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;

namespace DrillingCore.Application.FLHA.Commands
{
    public class CreateFLHAFormHandler : IRequestHandler<CreateFLHAFormCommand, int>
    {
        private readonly IFormRepository _formRepository;
        private readonly IFlhaRepository _flhaRepository;

        public CreateFLHAFormHandler(IFormRepository formRepository, IFlhaRepository flhaRepository)
        {
            _formRepository = formRepository;
            _flhaRepository = flhaRepository;
        }

        public async Task<int> Handle(CreateFLHAFormCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Step 1: Create ProjectForm (base form)
            var projectForm = new ProjectForm
            {
                ProjectId = dto.ProjectId,
                FormTypeId = dto.FormTypeId,
                CreatorId = dto.CreatorId,
                DateFilled =  dto.DateFilled,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                OtherComments = dto.OtherComments,
                CrewName = "",        // optional
                UnitNumber = "",      // optional
                Status = "Pending",

                FormParticipants = dto.Participants
            .Select(p => new FormParticipant
            {
                ParticipantId = p.ParticipantId
            }).ToList(),

                FormSignatures = dto.Participants
            .Where(p => !string.IsNullOrEmpty(p.SignatureUrl))
            .Select(p => new FormSignature
            {
                ParticipantId = p.ParticipantId,
                SignatureUrl = p.SignatureUrl
            }).ToList()
            };

            await _formRepository.AddProjectFormAsync(projectForm, cancellationToken);

            // Step 2: Create FLHAForm linked to ProjectForm
            var flhaForm = new FLHAForm
            {
                ProjectFormId = projectForm.Id,
                TaskDescription = dto.TaskDescription,
                Hazards = dto.Hazards.Select(h => new FLHAFormHazard
                {
                    HazardText = h.HazardText,
                    ControlMeasures = h.ControlMeasures,
                    HazardTemplateId = h.HazardTemplateId
                }).ToList()
            };

            await _flhaRepository.AddFlhaFormAsync(flhaForm, cancellationToken);

            return projectForm.Id;
        }
    }
}
