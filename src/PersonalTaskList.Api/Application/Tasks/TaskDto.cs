using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Application.Tasks;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? CompletedAt)
{
    public static TaskDto FromTask(TaskItem task)
    {
        return new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted,
            task.CreatedAt,
            task.UpdatedAt,
            task.CompletedAt);
    }
}
