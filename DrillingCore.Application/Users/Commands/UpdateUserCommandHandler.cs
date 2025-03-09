using DrillingCore.Application.Exceptions;
using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Application.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            user.Username = request.Username;
            // Пароль не обновляется здесь (если не указан отдельный сброс)
            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Mobile = request.Mobile;
            user.RoleId = request.RoleId;
            user.IsActive = request.IsActive; // Обновляем статус

            await _userRepository.UpdateUserAsync(user);
            return user.Id;
        }
    }
}
