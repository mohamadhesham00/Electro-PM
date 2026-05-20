using FluentValidation;

namespace Application.Features.Projects.Commands.Create
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
