namespace DrillingCore.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навигационное свойство для пользователей с данной ролью
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
