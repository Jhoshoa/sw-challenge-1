namespace PersonalTaskList.Api.Domain.Tasks;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken cancellationToken);

    Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    void Remove(TaskItem task);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
