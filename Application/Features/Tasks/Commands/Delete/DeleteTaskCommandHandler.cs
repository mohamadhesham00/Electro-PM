using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tasks.Commands.Delete
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IGenericRepository<TaskItem> _taskRepository;
        private readonly IGenericRepository<Project> _projectRepository;

        public DeleteTaskCommandHandler(IGenericRepository<TaskItem> taskRepository, IGenericRepository<Project> projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
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

            _taskRepository.Delete(task);
            await _taskRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
