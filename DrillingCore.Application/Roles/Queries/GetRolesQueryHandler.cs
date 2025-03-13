using MediatR;
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Roles.Queries
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            });
        }
    }
}
