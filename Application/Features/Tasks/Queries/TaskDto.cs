namespace Application.Features.Tasks.Queries
{
    public record TaskDto(
        Guid Id,
        string Title,
        string Description,
        string Status,
        DateTime? DueDate,
        string Priority,
        Guid ProjectId
    );
}
