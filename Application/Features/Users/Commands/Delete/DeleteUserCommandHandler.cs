using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Commands.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IGenericRepository<User> _userRepository;

        public DeleteUserCommandHandler(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Prevent admin from deleting themselves
            if (request.UserIdToDelete == request.AdminId)
            {
                throw new InvalidOperationException("You cannot delete your own account.");
            }

            // Verify user to delete exists
            var userToDelete = await _userRepository.GetByIdAsync(request.UserIdToDelete);
            if (userToDelete == null)
            {
                throw new NotFoundException("User not found.");
            }

            _userRepository.Delete(userToDelete);
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
