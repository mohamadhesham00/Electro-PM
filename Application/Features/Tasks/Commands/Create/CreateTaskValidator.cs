using FluentValidation;

namespace Application.Features.Tasks.Commands.Create
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(100).WithMessage("Task title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => new[] { "Pending", "InProgress", "Completed", "OnHold", "Cancelled" }.Contains(s))
                .WithMessage("Status must be one of: Pending, InProgress, Completed, OnHold, Cancelled.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .Must(p => new[] { "Low", "Medium", "High", "Critical" }.Contains(p))
                .WithMessage("Priority must be one of: Low, Medium, High, Critical.");
        }
    }
}
