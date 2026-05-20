using Microsoft.EntityFrameworkCore;

namespace PersonalTaskList.Api.Data;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<Models.Task> Tasks => Set<Models.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Task>(entity =>
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
