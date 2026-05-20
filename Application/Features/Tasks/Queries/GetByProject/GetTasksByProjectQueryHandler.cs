using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Tasks.Queries.GetByProject
{
    public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, List<TaskDto>>
    {
        private readonly IGenericRepository<TaskItem> _taskRepository;
        private readonly IGenericRepository<Project> _projectRepository;

        public GetTasksByProjectQueryHandler(IGenericRepository<TaskItem> taskRepository, IGenericRepository<Project> projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<List<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            // Verify project exists and belongs to the user
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null || project.OwnerId != request.UserId)
            {
                throw new NotFoundException("Project not found or you don't have access to it.");
            }

            var tasks = await _taskRepository.FindAsync(t => t.ProjectId == request.ProjectId);

            return tasks.Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.Priority,
                t.ProjectId
            )).ToList();
        }
    }
}
