using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly IGenericRepository<User> _userRepository;

        public RegisterUserCommandHandler(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // MediatR forces this exact method signature
        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUsers = await _userRepository.FindAsync(u => u.Email == request.Email);
            if (existingUsers.Any()) throw new AlreadyExistsException("Email taken.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User { Id = Guid.NewGuid(), Email = request.Email, Password= hashedPassword };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}
