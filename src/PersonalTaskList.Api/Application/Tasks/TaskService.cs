using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Application.Tasks;

public class TaskService(ITaskRepository taskRepository)
{
    public async Task<IReadOnlyList<TaskDto>> ListAsync(CancellationToken cancellationToken)
    {
        var tasks = await taskRepository.ListAsync(cancellationToken);

        return tasks
            .Select(TaskDto.FromTask)
            .ToList();
    }

    public async Task<TaskDto> CreateAsync(
        string title,
        string? description,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var task = TaskItem.Create(Guid.NewGuid(), title, description, now);

        await taskRepository.AddAsync(task, cancellationToken);
        await taskRepository.SaveChangesAsync(cancellationToken);

        return TaskDto.FromTask(task);
    }

    public async Task<TaskDto?> UpdateAsync(
        Guid id,
        string title,
        string? description,
        CancellationToken cancellationToken)
    {
        var task = await taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.UpdateDetails(title, description, DateTimeOffset.UtcNow);
        await taskRepository.SaveChangesAsync(cancellationToken);

        return TaskDto.FromTask(task);
    }

    public async Task<TaskDto?> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Complete(DateTimeOffset.UtcNow);
        await taskRepository.SaveChangesAsync(cancellationToken);

        return TaskDto.FromTask(task);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskRepository.FindByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        taskRepository.Remove(task);
        await taskRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
