using DrillingCore.Application.Forms.Commands;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;

public class CreateDrillInspectionHandler : IRequestHandler<CreateDrillInspectionCommand, int>
{
    private readonly IFormRepository _formRepository;

    public CreateDrillInspectionHandler(IFormRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<int> Handle(CreateDrillInspectionCommand request, CancellationToken cancellationToken)
    {
        var form = new ProjectForm
        {
            ProjectId = request.ProjectId,
            FormTypeId = 2,
            CreatorId = request.CreatorId,
            CrewName = request.CrewName,
            UnitNumber = request.UnitNumber,
            DateFilled = request.DateFilled,
            OtherComments = request.OtherComments
        };

        var checklist = request.ChecklistResponses.Select(x => new FormChecklistResponse
        {
            ChecklistItemId = x.ChecklistItemId,
            Response = x.Response
        }).ToList();

        var participants = request.Participants.Select(x => new FormParticipant
        {
            ParticipantId = x.ParticipantId,
            AttachDate = DateTime.UtcNow,
            Signature = x.Signature
        }).ToList();

        var photos = request.Photos.Select(x => new FormPhoto
        {
            PhotoUrl = x.PhotoUrl,
            CreatedDate = DateTime.UtcNow
        }).ToList();

        return await _formRepository.CreateDrillInspectionAsync(form, checklist, participants, photos, cancellationToken);
    }
}

