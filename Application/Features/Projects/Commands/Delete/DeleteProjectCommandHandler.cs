using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Commands.Delete
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public DeleteProjectCommandHandler(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
