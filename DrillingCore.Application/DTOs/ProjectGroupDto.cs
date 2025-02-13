using System.Collections.Generic;

namespace DrillingCore.Application.DTOs
{
    public class ProjectGroupDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    }
}
