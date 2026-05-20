using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Presentation.Dtos;

public class TaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public static TaskResponse FromTaskEntityToDto(TaskItem taskEntity)
    {
        return new TaskResponse
        {
            Id = taskEntity.Id,
            Title = taskEntity.Title,
            Description = taskEntity.Description,
            IsCompleted = taskEntity.IsCompleted,
            CreatedAt = taskEntity.CreatedAt,
            UpdatedAt = taskEntity.UpdatedAt,
            CompletedAt = taskEntity.CompletedAt
        };
    }
}
