using MediatR;

namespace Application.Features.Projects.Queries.GetAll
{
    public record GetAllProjectsQuery(Guid UserId) : IRequest<List<ProjectDto>>;
}
