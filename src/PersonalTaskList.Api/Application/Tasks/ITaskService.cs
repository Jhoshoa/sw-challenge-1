using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Application.Tasks;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken cancellationToken);

    Task<TaskItem> CreateAsync(
        string title,
        string? description,
        CancellationToken cancellationToken);

    Task<TaskItem?> UpdateAsync(
        Guid id,
        string title,
        string? description,
        CancellationToken cancellationToken);

    Task<TaskItem?> CompleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
