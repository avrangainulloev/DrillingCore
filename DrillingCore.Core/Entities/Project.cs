// Core/Entities/Project.cs
namespace DrillingCore.Core.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Client { get; set; } = null!;
        public bool HasCampOrHotel { get; set; }

        // Новый внешний ключ и навигационное свойство
        public int StatusId { get; set; }
        public ProjectStatus Status { get; set; } = null!;
    }

}
