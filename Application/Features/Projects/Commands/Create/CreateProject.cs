using MediatR;

namespace Application.Features.Projects.Commands.Create
{
    public record CreateProjectCommand(string Name, string Description, Guid userId) : IRequest<Guid>;
}
