// Application/DTOs/ProjectDto.cs
namespace DrillingCore.Application.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Client { get; set; } = null!;
        public bool HasCampOrHotel { get; set; }
        public string Status { get; set; } = null!;
        public int StatusId { get; set; }
    }
}
