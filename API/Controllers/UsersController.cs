using Application.Common.Models;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Queries;
using Application.Features.Users.Queries.GetAll;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// User management endpoints for administrators
    /// </summary>
    /// <remarks>
    /// These endpoints are restricted to administrators only.
    /// Allows listing all users and managing user accounts.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// List all users (Admin only)
        /// </summary>
        /// <remarks>
        /// Retrieves a list of all users in the system.
        /// **Restricted to:** Administrators only
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all users with their roles</returns>
        /// <response code="200">Users retrieved successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<List<UserDto>>.Success(users, "Users retrieved successfully!"));
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        /// <remarks>
        /// Permanently deletes a user account from the system.
        /// **Restrictions:**
        /// - Only administrators can delete users
        /// - Administrators cannot delete their own account
        /// 
        /// **Example:**
        /// ```
        /// DELETE /api/users/550e8400-e29b-41d4-a716-446655440002
        /// ```
        /// </remarks>
        /// <param name="userId">User ID (GUID) to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="401">Unauthorized - valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required or attempting self-deletion</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{userId}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = HttpContext.Items["CurrentUser"] as User;

            if (user == null)
            {
                return Unauthorized(new { message = "User context could not be resolved." });
            }

            var command = new DeleteUserCommand(userId, user.Id);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.Success(null, "User deleted successfully!"));
        }
    }
}
