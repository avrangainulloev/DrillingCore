using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Username= request.Username,
                PasswordHash = request.Password,
                FullName = request.FullName,
                Email = request.Email,
                Mobile = request.Mobile
            };

            await _userRepository.AddAsync(user);
            return user.Id;
        }
    }
}
