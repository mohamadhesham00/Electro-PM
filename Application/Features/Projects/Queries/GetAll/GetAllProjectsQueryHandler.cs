using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Queries.GetAll
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public GetAllProjectsQueryHandler(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.FindAsync(p => p.OwnerId == request.UserId);

            return projects.Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.OwnerId
            )).ToList();
        }
    }
}
