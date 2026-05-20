using Application.Common.Models;
using Application.Features.Tasks.Commands.Create;
using Application.Features.Tasks.Commands.Update;
using Application.Features.Tasks.Commands.Delete;
using Application.Features.Tasks.Queries;
using Application.Features.Tasks.Queries.GetByProject;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Task management endpoints for creating, reading, updating, and deleting tasks
    /// </summary>
    /// <remarks>
    /// Tasks are scoped to projects, and users can only manage tasks in their own projects.
    /// Supported task statuses: Pending, InProgress, Completed, OnHold, Cancelled
    /// Supported task priorities: Low, Medium, High, Critical
    /// </remarks>
    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    [Produces("application/json")]
    public class TasksController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Create a new task in a project
        /// </summary>
        /// <remarks>
        /// Creates a new task within a specific project.
        /// The project must belong to the authenticated user.
        /// 
        /// **Task Statuses:** Pending, InProgress, Completed, OnHold, Cancelled
        /// **Task Priorities:** Low, Medium, High, Critical
        /// 
        /// **Example:**
        /// ```
        /// POST /api/projects/{projectId}/tasks
        /// {
        ///   "title": "Design Database Schema",
        ///   "description": "Create normalized database design for user management",
        ///   "status": "Pending",
        ///   "dueDate": "2025-06-30",
        ///   "priority": "High"
        /// }
        /// ```
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="request">Task creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created task ID</returns>
        /// <response code="200">Task created successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Project not found or access denied</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            Guid projectId,
            [FromBody] CreateTaskRequest request,
            CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new CreateTaskCommand(
                projectId,
                request.Title,
                request.Description,
                request.Status,
                request.DueDate,
                request.Priority,
                user.Id
            );

            var taskId = await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(new { id = taskId }, "Task created successfully!"));
        }

        /// <summary>
        /// Retrieve all tasks in a project
        /// </summary>
        /// <remarks>
        /// Returns all tasks for a specific project owned by the authenticated user.
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks in the project</returns>
        /// <response code="200">Tasks retrieved successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Project not found or access denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<TaskDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var query = new GetTasksByProjectQuery(projectId, user.Id);
            var tasks = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<List<TaskDto>>.Success(tasks, "Tasks retrieved successfully!"));
        }

        /// <summary>
        /// Update a task (supports partial updates)
        /// </summary>
        /// <remarks>
        /// Updates one or more task fields. All fields are optional.
        /// 
        /// **Example - Update only status:**
        /// ```
        /// PATCH /api/projects/{projectId}/tasks/{taskId}
        /// {
        ///   "status": "InProgress"
        /// }
        /// ```
        /// 
        /// **Example - Update multiple fields:**
        /// ```
        /// PATCH /api/projects/{projectId}/tasks/{taskId}
        /// {
        ///   "title": "Updated Task Title",
        ///   "status": "Completed",
        ///   "priority": "Critical"
        /// }
        /// ```
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="taskId">Task ID (GUID)</param>
        /// <param name="request">Update request (all fields optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Task updated successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Task or project not found or access denied</response>
        /// <response code="400">Invalid request data</response>
        [HttpPatch("{taskId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(
            Guid projectId,
            Guid taskId,
            [FromBody] UpdateTaskRequest request,
            CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new UpdateTaskCommand(
                taskId,
                request.Title,
                request.Description,
                request.Status,
                request.DueDate,
                request.Priority,
                projectId,
                user.Id
            );
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(null, "Task updated successfully!"));
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <remarks>
        /// Permanently deletes a task from a project.
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="taskId">Task ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Task deleted successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Task or project not found or access denied</response>
        [HttpDelete("{taskId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            Guid projectId,
            Guid taskId,
            CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new DeleteTaskCommand(taskId, projectId, user.Id);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(null, "Task deleted successfully!"));
        }
    }

    /// <summary>
    /// Request model for creating a new task
    /// </summary>
    public record CreateTaskRequest(
        /// <summary>Task title (required)</summary>
        string Title,
        /// <summary>Task description (optional)</summary>
        string Description,
        /// <summary>Task status (required) - one of: Pending, InProgress, Completed, OnHold, Cancelled</summary>
        string Status,
        /// <summary>Task due date (optional)</summary>
        DateTime? DueDate,
        /// <summary>Task priority (required) - one of: Low, Medium, High, Critical</summary>
        string Priority
    );

    /// <summary>
    /// Request model for updating a task (all fields optional for partial updates)
    /// </summary>
    public record UpdateTaskRequest(
        /// <summary>Task title (optional) - omit to keep existing value</summary>
        string? Title = null,
        /// <summary>Task description (optional) - omit to keep existing value</summary>
        string? Description = null,
        /// <summary>Task status (optional) - one of: Pending, InProgress, Completed, OnHold, Cancelled</summary>
        string? Status = null,
        /// <summary>Task due date (optional) - omit to keep existing value</summary>
        DateTime? DueDate = null,
        /// <summary>Task priority (optional) - one of: Low, Medium, High, Critical</summary>
        string? Priority = null
    );
}
