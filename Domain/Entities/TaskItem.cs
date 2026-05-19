namespace Domain.Entities;

// I renamed it from Task to TaskItem to avoid conflict with System.Threading.Tasks.Task
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }

    public Project? Project { get; set; }
}
