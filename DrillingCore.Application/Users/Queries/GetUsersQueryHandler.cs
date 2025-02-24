// GetUsersQueryHandler.cs
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            // Получаем пользователей с фильтрами через новый метод
            var users = await _userRepository.GetUsersAsync(request.SearchTerm, request.RoleId);

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Mobile = u.Mobile,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? string.Empty
            });
        }
    }
}
