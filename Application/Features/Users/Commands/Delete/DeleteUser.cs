using MediatR;

namespace Application.Features.Users.Commands.Delete
{
    public record DeleteUserCommand(Guid UserIdToDelete, Guid AdminId) : IRequest<Unit>;
}
