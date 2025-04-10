using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        private readonly IUserRepository _repository;

        public GetUserByUsernameHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByUsernameAsync(request.Username);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role.Name
            };
        }
    }
}
