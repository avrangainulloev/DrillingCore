using System.Threading;
using System.Threading.Tasks;
using MediatR;
using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                // Depending on your error-handling policy you could throw a NotFoundException or simply return null.
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Mobile = user.Mobile,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive
            };
        }
    }
}
