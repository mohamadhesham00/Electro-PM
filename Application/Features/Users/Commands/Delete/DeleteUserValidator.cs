using FluentValidation;

namespace Application.Features.Users.Commands.Delete
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserIdToDelete)
                .NotEmpty().WithMessage("User ID to delete is required.");

            RuleFor(x => x.AdminId)
                .NotEmpty().WithMessage("Admin ID is required.");

            RuleFor(x => x)
                .Must(x => x.UserIdToDelete != x.AdminId)
                .WithMessage("Cannot delete your own account.");
        }
    }
}
