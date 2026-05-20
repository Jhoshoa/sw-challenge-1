using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Infrastructure.Persistence;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; } = null!;
}
