using MediatR;
using DrillingCore.Application.DTOs;
using System.Collections.Generic;

namespace DrillingCore.Application.Roles.Queries
{
    public class GetRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
    }
}
