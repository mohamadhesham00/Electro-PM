using Application.Common.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            // Map your C# Exceptions perfectly to HTTP Protocol Status Codes
            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,             // 404 Not Found
                NotFoundException => HttpStatusCode.NotFound,             // 404 Not Found
                AlreadyExistsException => HttpStatusCode.Conflict,           // 409 Conflict
                ArgumentException => HttpStatusCode.BadRequest,              // 400 Bad Request
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,  // 401 Unauthorized
                _ => HttpStatusCode.InternalServerError                      // 500 Server Error
            };

            // Inject the code into the actual HTTP transport network headers
            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Failure(
                errors: new List<string> { exception.Message },
                message: statusCode switch
                {
                    HttpStatusCode.NotFound => "The requested resource could not be found.",
                    HttpStatusCode.Conflict => "A data conflict occurred. The resource might already exist.",
                    HttpStatusCode.InternalServerError => "A critical server error occurred.",
                    _ => "A validation rule was violated."
                }
            );

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
