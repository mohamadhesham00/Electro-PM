using MediatR;

namespace Application.Features.Projects.Commands.Delete
{
    public record DeleteProjectCommand(Guid ProjectId, Guid UserId) : IRequest<Unit>;
}
