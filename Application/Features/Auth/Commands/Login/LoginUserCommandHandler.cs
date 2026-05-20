using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUserCommandHandler(IGenericRepository<User> userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.FindAsync(u => u.Email == request.Email);
            var user = users.FirstOrDefault();

            if (user == null) throw new NotFoundException("User not found.");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(user);
            return new LoginResponse(token, user.Email);
        }
    }
}
