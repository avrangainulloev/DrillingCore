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
            FormTypeId = request.FormTypeId,
            CreatorId = request.CreatorId,
            CrewName = request.CrewName,
            UnitNumber = request.UnitNumber,
            DateFilled = DateTime.SpecifyKind(request.DateFilled, DateTimeKind.Utc),
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
            AttachDate = DateTime.UtcNow
        }).ToList();

        // Фото и подписи теперь загружаются отдельно через отдельные команды, удалено из основного запроса

        return await _formRepository.CreateDrillInspectionAsync(form, checklist, participants, cancellationToken);
    }
}

