using FluentValidation;

namespace Application.Features.Projects.Commands.Update
{
    public class UpdateProjectValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required.");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
