using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tasks.Commands.Create
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IGenericRepository<TaskItem> _taskRepository;
        private readonly IGenericRepository<Project> _projectRepository;

        public CreateTaskCommandHandler(IGenericRepository<TaskItem> taskRepository, IGenericRepository<Project> projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                DueDate = request.DueDate,
                Priority = request.Priority,
                ProjectId = request.ProjectId
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return task.Id;
        }
    }
}
