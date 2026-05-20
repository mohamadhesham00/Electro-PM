using Domain.Entities;
using Domain.Repositories;
using System.Security.Claims;

namespace API.Middlewares
{
    public class CurrentUserMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IGenericRepository<User> userRepository)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var currentUser = await userRepository.GetByIdAsync(userId);

                    if (currentUser != null)
                    {
                        // 2. Store the full entity completely ready for your controller
                        context.Items["CurrentUser"] = currentUser;

                        // You can also still save just the ID if you need it separately
                        context.Items["UserId"] = userId;
                    }
                }
            }

            await _next(context);
        }
    }
}
