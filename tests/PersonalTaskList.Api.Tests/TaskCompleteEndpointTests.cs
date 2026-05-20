using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Presentation.Contracts;
using PersonalTaskList.Api.Domain.Tasks;
using PersonalTaskList.Api.Infrastructure.Persistence;

namespace PersonalTaskList.Api.Tests;

public class TaskCompleteEndpointTests
{
    [Fact]
    public async Task PatchComplete_WhenTaskIsIncomplete_ReturnsCompletedTask()
    {
        using var factory = new TaskApiFactory();
        var taskId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Tasks.Add(TaskItem.Create(taskId, "Incomplete task", null, createdAt));

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.PatchAsync($"/api/tasks/{taskId}/complete", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(body);
        Assert.True(body.IsCompleted);
        Assert.NotNull(body.CompletedAt);
        Assert.True(body.UpdatedAt > createdAt);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<TaskDbContext>();
        var savedTask = await verifyDbContext.Tasks.SingleAsync(task => task.Id == taskId);

        Assert.True(savedTask.IsCompleted);
        Assert.NotNull(savedTask.CompletedAt);
    }

    [Fact]
    public async Task PatchComplete_WhenTaskIsAlreadyCompleted_ReturnsCompletedTaskWithoutChangingCompletionTimestamp()
    {
        using var factory = new TaskApiFactory();
        var taskId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var completedAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            var task = TaskItem.Create(taskId, "Completed task", null, createdAt);
            task.Complete(completedAt);
            dbContext.Tasks.Add(task);

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.PatchAsync($"/api/tasks/{taskId}/complete", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(body);
        Assert.True(body.IsCompleted);
        Assert.Equal(completedAt, body.CompletedAt);
        Assert.Equal(completedAt, body.UpdatedAt);
    }

    [Fact]
    public async Task PatchComplete_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PatchAsync($"/api/tasks/{Guid.NewGuid()}/complete", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
