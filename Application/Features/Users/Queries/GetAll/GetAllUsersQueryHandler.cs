using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Queries.GetAll
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IGenericRepository<User> _userRepository;

        public GetAllUsersQueryHandler(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto(
                u.Id,
                u.Email,
                u.Role.ToString()
            )).ToList();
        }
    }
}
