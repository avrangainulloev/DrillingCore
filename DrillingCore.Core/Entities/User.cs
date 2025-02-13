namespace DrillingCore.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        // Для простоты пароль будем хранить в виде строки (в реальном проекте используйте хэширование!)
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Например: Administrator, ProjectManager, FieldEmployee
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }
}
