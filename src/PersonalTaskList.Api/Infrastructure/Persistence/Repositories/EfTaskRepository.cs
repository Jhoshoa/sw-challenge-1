using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Infrastructure.Persistence.Repositories;

public class EfTaskRepository(TaskDbContext dbContext) : ITaskRepository
{
    public async Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        await dbContext.Tasks.AddAsync(task, cancellationToken);
    }

    public void Remove(TaskItem task)
    {
        dbContext.Tasks.Remove(task);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
