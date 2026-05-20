using FluentValidation;

namespace Application.Features.Tasks.Commands.Update
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Task title cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Status)
                .Must(s => new[] { "Pending", "InProgress", "Completed", "OnHold", "Cancelled" }.Contains(s))
                .WithMessage("Status must be one of: Pending, InProgress, Completed, OnHold, Cancelled.")
                .When(x => !string.IsNullOrEmpty(x.Status));

            RuleFor(x => x.Priority)
                .Must(p => new[] { "Low", "Medium", "High", "Critical" }.Contains(p))
                .WithMessage("Priority must be one of: Low, Medium, High, Critical.")
                .When(x => !string.IsNullOrEmpty(x.Priority));
        }
    }
}
