using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
