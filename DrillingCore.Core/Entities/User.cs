namespace DrillingCore.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Вместо хранения строки, теперь указываем внешний ключ на роль
        public int RoleId { get; set; }
        public Role Role { get; set; }  // Навигационное свойство

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;

        // Новое поле для хранения статуса пользователя (по умолчанию активен)
        public bool IsActive { get; set; } = true;
    }
}
