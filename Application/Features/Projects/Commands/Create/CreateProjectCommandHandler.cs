using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Commands.Create
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public CreateProjectCommandHandler(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                OwnerId = request.userId,
                Tasks = new List<TaskItem>()
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            return project.Id;
        }
    }
}
