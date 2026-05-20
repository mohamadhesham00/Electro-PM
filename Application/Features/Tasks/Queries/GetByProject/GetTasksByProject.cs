using MediatR;

namespace Application.Features.Tasks.Queries.GetByProject
{
    public record GetTasksByProjectQuery(Guid ProjectId, Guid UserId) : IRequest<List<TaskDto>>;
}
