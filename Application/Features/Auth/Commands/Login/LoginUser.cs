using MediatR;

namespace Application.Features.Auth.Commands.Login
{
    public record LoginUserCommand(string Email, string Password) : IRequest<LoginResponse>;
}
