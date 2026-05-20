using Domain.Enums;

namespace Application.Features.Users.Queries
{
    public record UserDto(
        Guid Id,
        string Email,
        string Role
    );
}
