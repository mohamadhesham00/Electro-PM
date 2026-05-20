using Application.Extensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tasks.Commands.Update
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
    {
        private readonly IGenericRepository<TaskItem> _taskRepository;
        private readonly IGenericRepository<Project> _projectRepository;

        public UpdateTaskCommandHandler(IGenericRepository<TaskItem> taskRepository, IGenericRepository<Project> projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            // Verify task exists and belongs to the project
            var task = await _taskRepository.GetByIdAsync(request.TaskId);
            if (task == null || task.ProjectId != request.ProjectId)
            {
                throw new NotFoundException("Task not found or you don't have access to it.");
            }

            // Map To task entity 
            request.PatchInto(task);

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
