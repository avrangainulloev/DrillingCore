using MediatR;
using DrillingCore.Application.DTOs;
using System.Collections.Generic;

namespace DrillingCore.Application.Users.Queries
{
    public class GetAvailableUsersQuery : IRequest<IEnumerable<UserDto>>
    {
       
    }
}
