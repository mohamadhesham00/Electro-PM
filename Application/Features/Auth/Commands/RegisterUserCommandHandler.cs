using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands
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
            if (existingUsers.Any()) throw new Exception("Email taken.");

            var user = new User { Id = Guid.NewGuid(), Email = request.Email };
            await _userRepository.AddAsync(user);
        }
    }
}
