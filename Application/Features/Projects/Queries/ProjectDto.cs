namespace Application.Features.Projects.Queries
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        DateTime CreatedAt,
        Guid OwnerId
    );
}
