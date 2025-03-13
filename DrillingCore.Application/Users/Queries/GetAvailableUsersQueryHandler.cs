using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetAvailableUsersQueryHandler : IRequestHandler<GetAvailableUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetAvailableUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAvailableUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAvailableUsersAsync();

            // Преобразуем сущности в DTO (пример; можно адаптировать под свои нужды)
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Mobile = u.Mobile,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? string.Empty,
                IsActive = u.IsActive
            });
        }
    }
}
