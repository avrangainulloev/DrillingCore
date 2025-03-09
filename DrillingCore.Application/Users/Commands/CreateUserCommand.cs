using MediatR;

namespace DrillingCore.Application.Users.Commands
{
    // Команда возвращает Id созданного пользователя
    public class CreateUserCommand : IRequest<int>
    {
        public string Username {get;set;}
        public string Password {get;set;}
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int RoleId { get; set; } 
        public bool IsActive { get; set; }
    }
}
