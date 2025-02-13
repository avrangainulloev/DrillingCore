namespace DrillingCore.Core.Entities
{
    public class ProjectGroup
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        // Навигационное свойство – список участников группы
        // Коллекция участников
        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}
