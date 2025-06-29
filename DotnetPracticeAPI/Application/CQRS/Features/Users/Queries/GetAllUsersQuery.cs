using Application.DTOs;
using MediatR;

namespace Application.CQRS.Features.Users.Queries
{
    public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
}
