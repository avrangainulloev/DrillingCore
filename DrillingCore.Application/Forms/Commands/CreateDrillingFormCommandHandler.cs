namespace DrillingCore.Application.Forms.Commands
{
    using global::DrillingCore.Application.Interfaces;
    using global::DrillingCore.Core.Entities;
    using MediatR;

    namespace DrillingCore.Application.Forms.Handlers
    {
        public class CreateDrillingFormCommandHandler : IRequestHandler<CreateDrillingFormCommand, int>
        {
            private readonly IFormRepository _repository;

            public CreateDrillingFormCommandHandler(IFormRepository repository)
            {
                _repository = repository;
            }

            public async Task<int> Handle(CreateDrillingFormCommand request, CancellationToken cancellationToken)
            {
                var form = new ProjectForm
                {
                    ProjectId = request.ProjectId,
                    CreatorId = request.CreatorId,
                    DateFilled = request.DateFilled,
                    FormTypeId = 5, // "Drilling"
                    CrewName = request.CrewName,
                    UnitNumber = request.UnitNumber,
                    OtherComments = request.OtherComments,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };

                var drilling = new DrillingForm
                {
                    NumberOfWells = request.NumberOfWells,
                    TotalMeters = request.TotalMeters
                };

                var participants = request.Participants.Select(p => new FormParticipant
                {
                    ParticipantId = p.ParticipantId
                }).ToList();

                return await _repository.CreateDrillingFormAsync(form, drilling, participants, cancellationToken);
            }
        }
    }

}
