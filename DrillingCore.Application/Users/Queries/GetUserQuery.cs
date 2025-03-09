using MediatR;
using DrillingCore.Application.DTOs;

namespace DrillingCore.Application.Users.Queries
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public int UserId { get; }

        public GetUserQuery(int userId)
        {
            UserId = userId;
        }
    }
}
