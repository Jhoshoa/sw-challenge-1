using PersonalTaskList.Api.Application.Tasks;

namespace PersonalTaskList.Api.Presentation.Contracts;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? CompletedAt)
{
    public static TaskResponse FromTask(TaskDto task)
    {
        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.IsCompleted,
            task.CreatedAt,
            task.UpdatedAt,
            task.CompletedAt);
    }
}
