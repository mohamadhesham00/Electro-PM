using MediatR;

namespace Application.Features.Tasks.Commands.Create
{
    public record CreateTaskCommand(
        Guid ProjectId,
        string Title,
        string Description,
        string Status,
        DateTime? DueDate,
        string Priority,
        Guid UserId
    ) : IRequest<Guid>;
}
