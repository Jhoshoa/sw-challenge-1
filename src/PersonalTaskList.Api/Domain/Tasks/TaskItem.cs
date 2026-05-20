namespace PersonalTaskList.Api.Domain.Tasks;

public class TaskItem
{
    private TaskItem()
    {
        Title = string.Empty;
    }

    private TaskItem(Guid id, string title, string? description, DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        IsCompleted = false;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        CompletedAt = null;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static TaskItem Create(Guid id, string title, string? description, DateTimeOffset createdAt)
    {
        return new TaskItem(id, NormalizeTitle(title), NormalizeDescription(description), createdAt);
    }

    public void UpdateDetails(string title, string? description, DateTimeOffset updatedAt)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        UpdatedAt = updatedAt;
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (IsCompleted)
        {
            return;
        }

        IsCompleted = true;
        CompletedAt = completedAt;
        UpdatedAt = completedAt;
    }

    private static string NormalizeTitle(string title)
    {
        return title.Trim();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description;
    }
}
