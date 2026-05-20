using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Data;

namespace PersonalTaskList.Api.Tests;

public class PersistenceModelTests
{
    [Fact]
    public void TaskDbContextUsesSqliteProvider()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new TaskDbContext(options);

        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", dbContext.Database.ProviderName);
    }

    [Fact]
    public void TaskEntityMapsToTasksTableWithDocumentedColumns()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new TaskDbContext(options);
        var entityType = dbContext.Model.FindEntityType(typeof(Api.Models.Task));

        Assert.NotNull(entityType);
        Assert.Equal("tasks", entityType.GetTableName());

        var columnNames = entityType
            .GetProperties()
            .Select(property => property.GetColumnName())
            .ToHashSet();

        Assert.True(columnNames.SetEquals(
        [
            "id",
            "title",
            "description",
            "isCompleted",
            "createdAt",
            "updatedAt",
            "completedAt"
        ]));
    }
}
