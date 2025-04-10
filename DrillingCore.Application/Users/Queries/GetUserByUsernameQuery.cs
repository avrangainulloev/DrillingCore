using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUserByUsernameQuery : IRequest<UserDto>
    {
        public string Username { get; set; }
    }
        
}
