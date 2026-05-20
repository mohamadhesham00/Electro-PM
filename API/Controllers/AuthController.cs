using Application.Common.Models;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Authentication endpoints for user registration and login
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <remarks>
        /// Creates a new user account with email and password.
        /// 
        /// **Requirements:**
        /// - Email must be unique
        /// - Email must be valid format
        /// - Password must be at least 6 characters long
        /// 
        /// **Example:**
        /// ```
        /// POST /api/auth/register
        /// {
        ///   "email": "user@example.com",
        ///   "password": "SecurePassword123"
        /// }
        /// ```
        /// </remarks>
        /// <param name="command">Registration command with email and password</param>
        /// <returns>Success message if registration is successful</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid input or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            await _mediator.Send(command);

            return Ok(ApiResponse<object>.Success(null, "User registered successfully."));
        }

        /// <summary>
        /// Authenticate user and receive JWT token
        /// </summary>
        /// <remarks>
        /// Authenticates user credentials and returns a JWT token for subsequent authenticated requests.
        /// 
        /// **Token validity:**
        /// - Token expires based on server configuration (typically 1 hour)
        /// - Include token in Authorization header for authenticated requests
        /// - Format: `Authorization: Bearer {token}`
        /// 
        /// **Example:**
        /// ```
        /// POST /api/auth/login
        /// {
        ///   "email": "user@example.com",
        ///   "password": "SecurePassword123"
        /// }
        /// ```
        /// </remarks>
        /// <param name="command">Login command with email and password</param>
        /// <returns>JWT token and user email on successful login</returns>
        /// <response code="200">Login successful with JWT token</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(ApiResponse<LoginResponse>.Success(response, "User logged in successfully."));
        }
    }
}
