using Application.Common.Models;
using Application.Features.Projects.Commands.Create;
using Application.Features.Projects.Commands.Update;
using Application.Features.Projects.Commands.Delete;
using Application.Features.Projects.Queries.GetAll;
using Application.Features.Projects.Queries.GetById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Projects.Queries;

namespace API.Controllers
{
    /// <summary>
    /// Project management endpoints for CRUD operations
    /// </summary>
    /// <remarks>
    /// All project operations are scoped to the authenticated user.
    /// Users can only access and modify their own projects.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProjectsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <remarks>
        /// Creates a new project for the authenticated user.
        /// 
        /// **Requirements:**
        /// - Project name is required
        /// - Description is optional
        /// - User must be authenticated
        /// 
        /// **Example:**
        /// ```
        /// POST /api/projects
        /// {
        ///   "name": "Q1 2025 Initiative",
        ///   "description": "Strategic project for Q1 quarter"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Project creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created project ID</returns>
        /// <response code="200">Project created successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }
            var command = new CreateProjectCommand(request.Name, request.Description, user.Id); 
            var projectId = await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(new { id = projectId }, "Project created successfully!"));
        }

        /// <summary>
        /// Retrieve all projects for the authenticated user
        /// </summary>
        /// <remarks>
        /// Returns a list of all projects owned by the authenticated user.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of projects</returns>
        /// <response code="200">Projects retrieved successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ProjectDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var query = new GetAllProjectsQuery(user.Id);
            var projects = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<List<ProjectDto>>.Success(projects, "Projects retrieved successfully!"));
        }

        /// <summary>
        /// Retrieve a specific project by ID
        /// </summary>
        /// <remarks>
        /// Returns a single project if it exists and belongs to the authenticated user.
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Project details</returns>
        /// <response code="200">Project retrieved successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Project not found or access denied</response>
        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid projectId, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var query = new GetProjectByIdQuery(projectId, user.Id);
            var project = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<ProjectDto>.Success(project, "Project retrieved successfully!"));
        }

        /// <summary>
        /// Update an existing project (supports partial updates)
        /// </summary>
        /// <remarks>
        /// Updates a project's name and/or description.
        /// Only one or both fields can be updated in a single request.
        /// 
        /// **Example - Update only name:**
        /// ```
        /// PUT /api/projects/{projectId}
        /// {
        ///   "name": "Updated Project Name"
        /// }
        /// ```
        /// 
        /// **Example - Update only description:**
        /// ```
        /// PUT /api/projects/{projectId}
        /// {
        ///   "description": "Updated description"
        /// }
        /// ```
        /// 
        /// **Example - Update both:**
        /// ```
        /// PUT /api/projects/{projectId}
        /// {
        ///   "name": "New Name",
        ///   "description": "New description"
        /// }
        /// ```
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="request">Update request (all fields optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Project updated successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Project not found or access denied</response>
        /// <response code="400">Invalid request data</response>
        [HttpPut("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid projectId, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new UpdateProjectCommand(projectId, request.Name, request.Description, user.Id);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(null, "Project updated successfully!"));
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <remarks>
        /// Permanently deletes a project and all associated tasks.
        /// Only the project owner can delete their projects.
        /// </remarks>
        /// <param name="projectId">Project ID (GUID)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="200">Project deleted successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="404">Project not found or access denied</response>
        [HttpDelete("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid projectId, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new DeleteProjectCommand(projectId, user.Id);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(null, "Project deleted successfully!"));
        }
    }

    /// <summary>
    /// Request model for creating a new project
    /// </summary>
    public record CreateProjectRequest(
        /// <summary>Project name (required)</summary>
        string Name,
        /// <summary>Project description (optional)</summary>
        string Description
    );

    /// <summary>
    /// Request model for updating a project (all fields optional)
    /// </summary>
    public record UpdateProjectRequest(
        /// <summary>Project name (optional) - omit to keep existing value</summary>
        string? Name = null,
        /// <summary>Project description (optional) - omit to keep existing value</summary>
        string? Description = null
    );
}
