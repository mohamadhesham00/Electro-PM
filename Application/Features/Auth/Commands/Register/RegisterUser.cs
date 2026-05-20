using MediatR;

namespace Application.Features.Auth.Commands.Register
{
    public record RegisterUserCommand(string Email, string Password) : IRequest;
}
