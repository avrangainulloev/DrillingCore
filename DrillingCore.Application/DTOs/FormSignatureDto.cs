namespace DrillingCore.Application.DTOs
{
    public class FormSignatureDto
    {
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = default!;
        public string Signature { get; set; } = default!;
    }
}
