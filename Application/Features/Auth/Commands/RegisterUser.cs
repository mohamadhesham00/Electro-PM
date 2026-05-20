using MediatR;

namespace Application.Features.Auth.Commands
{
    public record RegisterUserCommand(string Email, string Password) : IRequest;
}
