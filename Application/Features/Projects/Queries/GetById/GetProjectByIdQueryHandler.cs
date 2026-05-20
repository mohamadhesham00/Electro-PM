using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Queries.GetById
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public GetProjectByIdQueryHandler(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            return new ProjectDto(
                project.Id,
                project.Name,
                project.Description,
                project.CreatedAt,
                project.OwnerId
            );
        }
    }
}
