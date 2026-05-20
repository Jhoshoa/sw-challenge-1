using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Presentation.Contracts;
using PersonalTaskList.Api.Domain.Tasks;
using PersonalTaskList.Api.Infrastructure.Persistence;

namespace PersonalTaskList.Api.Tests;

public class TaskEditEndpointTests
{
    [Fact]
    public async Task PutTask_WhenTaskExistsAndInputIsValid_ReturnsUpdatedTask()
    {
        using var factory = new TaskApiFactory();
        var taskId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Tasks.Add(TaskItem.Create(taskId, "Buy groceries", "Milk, eggs, bread", createdAt));

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/tasks/{taskId}", new
        {
            title = "Buy groceries and coffee",
            description = "Milk, eggs, bread, coffee"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(body);
        Assert.Equal(taskId, body.Id);
        Assert.Equal("Buy groceries and coffee", body.Title);
        Assert.Equal("Milk, eggs, bread, coffee", body.Description);
        Assert.True(body.UpdatedAt > createdAt);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<TaskDbContext>();
        var savedTask = await verifyDbContext.Tasks.SingleAsync(task => task.Id == taskId);

        Assert.Equal("Buy groceries and coffee", savedTask.Title);
        Assert.Equal("Milk, eggs, bread, coffee", savedTask.Description);
    }

    [Fact]
    public async Task PutTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}", new
        {
            title = "Missing task",
            description = "No matching task"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutTask_WithBlankTitle_ReturnsBadRequestAndDoesNotUpdateTask()
    {
        using var factory = new TaskApiFactory();
        var taskId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Tasks.Add(TaskItem.Create(taskId, "Original title", "Original description", createdAt));

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/tasks/{taskId}", new
        {
            title = "   ",
            description = "Updated description"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<TaskDbContext>();
        var savedTask = await verifyDbContext.Tasks.SingleAsync(task => task.Id == taskId);

        Assert.Equal("Original title", savedTask.Title);
        Assert.Equal("Original description", savedTask.Description);
        Assert.Equal(createdAt, savedTask.UpdatedAt);
    }
}
