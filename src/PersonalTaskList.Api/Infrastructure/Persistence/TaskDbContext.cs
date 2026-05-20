using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Domain.Tasks;

namespace PersonalTaskList.Api.Infrastructure.Persistence;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(task => task.Title)
                .HasColumnName("title")
                .IsRequired();

            entity.Property(task => task.Description)
                .HasColumnName("description");

            entity.Property(task => task.IsCompleted)
                .HasColumnName("isCompleted")
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .HasColumnName("createdAt")
                .IsRequired();

            entity.Property(task => task.UpdatedAt)
                .HasColumnName("updatedAt")
                .IsRequired();

            entity.Property(task => task.CompletedAt)
                .HasColumnName("completedAt");
        });
    }
}
