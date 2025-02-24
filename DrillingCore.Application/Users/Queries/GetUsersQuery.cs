using DrillingCore.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public string? SearchTerm { get; set; }
        public int? RoleId { get; set; }
    }
}
