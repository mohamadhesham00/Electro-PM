using FluentValidation;

namespace Application.Features.Projects.Commands.Delete
{
    public class DeleteProjectValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required.");
        }
    }
}
