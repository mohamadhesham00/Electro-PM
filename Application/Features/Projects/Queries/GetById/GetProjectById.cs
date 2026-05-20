using MediatR;

namespace Application.Features.Projects.Queries.GetById
{
    public record GetProjectByIdQuery(Guid ProjectId, Guid UserId) : IRequest<ProjectDto>;
}
