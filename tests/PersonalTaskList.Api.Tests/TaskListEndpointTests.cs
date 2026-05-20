using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Presentation.Dtos;
using PersonalTaskList.Api.Domain.Tasks;
using PersonalTaskList.Api.Infrastructure.Persistence;

namespace PersonalTaskList.Api.Tests;

public class TaskListEndpointTests
{
    [Fact]
    public async Task GetTasks_WhenNoTasksExist_ReturnsEmptyArray()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<TaskResponse>>();

        Assert.NotNull(body);
        Assert.Empty(body);
    }

    [Fact]
    public async Task GetTasks_WhenTasksExist_ReturnsStoredTasks()
    {
        using var factory = new TaskApiFactory();

        var firstTaskId = Guid.NewGuid();
        var secondTaskId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

            var firstTask = TaskItem.Create(firstTaskId, "First task", "First description", now);
            var secondTask = TaskItem.Create(secondTaskId, "Second task", null, now.AddMinutes(1));
            secondTask.Complete(now.AddMinutes(2));

            dbContext.Tasks.AddRange(firstTask, secondTask);

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<TaskResponse>>();

        Assert.NotNull(body);
        Assert.Equal(2, body.Count);

        var first = Assert.Single(body, task => task.Id == firstTaskId);
        Assert.Equal("First task", first.Title);
        Assert.Equal("First description", first.Description);
        Assert.False(first.IsCompleted);
        Assert.Null(first.CompletedAt);

        var second = Assert.Single(body, task => task.Id == secondTaskId);
        Assert.Equal("Second task", second.Title);
        Assert.Null(second.Description);
        Assert.True(second.IsCompleted);
        Assert.NotNull(second.CompletedAt);
    }
}
