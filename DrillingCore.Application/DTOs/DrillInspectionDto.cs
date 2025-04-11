namespace DrillingCore.Application.DTOs
{
    public class DrillInspectionDto
    {
        public int Id { get; set; }
        public string CrewName { get; set; } = default!;
        public string UnitNumber { get; set; } = default!;
        public DateTime DateFilled { get; set; }
        public string? OtherComments { get; set; }
        public int ProjectId { get; set; }

        public List<ChecklistResponseDto> ChecklistResponses { get; set; } = new();
        public List<FormParticipantDto> Participants { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new();
        public List<FormSignatureDto> Signatures { get; set; } = new();
    }

}
