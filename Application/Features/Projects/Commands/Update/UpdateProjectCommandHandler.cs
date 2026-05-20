using Application.Extensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Commands.Update
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
    {
        private readonly IGenericRepository<Project> _projectRepository;

        public UpdateProjectCommandHandler(IGenericRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);

            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            // Map to project entity
            request.PatchInto(project);

            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
