using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Infrastructure.Persistence.Repositories;

public class EfTaskRepository : ITaskRepository
{
    private readonly TaskDbContext _dbContext;

    public EfTaskRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskItem>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        await _dbContext.Tasks.AddAsync(task, cancellationToken);
    }

    public void Remove(TaskItem task)
    {
        _dbContext.Tasks.Remove(task);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
