using MediatR;

namespace Application.Features.Projects.Commands.Update
{
    public record UpdateProjectCommand(Guid ProjectId, string? Name, string? Description, Guid UserId) : IRequest<Unit>;
}
