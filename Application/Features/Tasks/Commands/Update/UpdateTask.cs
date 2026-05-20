using MediatR;

namespace Application.Features.Tasks.Commands.Update
{
    public record UpdateTaskCommand(
        Guid TaskId,
        string? Title,
        string? Description,
        string? Status,
        DateTime? DueDate,
        string? Priority,
        Guid ProjectId,
        Guid UserId
    ) : IRequest<Unit>;
}
