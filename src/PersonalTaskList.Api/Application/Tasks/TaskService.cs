using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Application.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken cancellationToken)
    {
        return await _taskRepository.ListAsync(cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(
        string title,
        string? description,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var taskEntity = FromTaskInputToEntity(title, description, now);

        await _taskRepository.AddAsync(taskEntity, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return taskEntity;
    }

    public async Task<TaskItem?> UpdateAsync(
        Guid id,
        string title,
        string? description,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.UpdateDetails(title, description, DateTimeOffset.UtcNow);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<TaskItem?> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Complete(DateTimeOffset.UtcNow);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        _taskRepository.Remove(task);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static TaskItem FromTaskInputToEntity(
        string title,
        string? description,
        DateTimeOffset createdAt)
    {
        return TaskItem.Create(Guid.NewGuid(), title, description, createdAt);
    }
}
