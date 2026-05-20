using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Contracts;
using PersonalTaskList.Api.Data;

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

            dbContext.Tasks.AddRange(
                new Api.Models.Task
                {
                    Id = firstTaskId,
                    Title = "First task",
                    Description = "First description",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new Api.Models.Task
                {
                    Id = secondTaskId,
                    Title = "Second task",
                    Description = null,
                    IsCompleted = true,
                    CreatedAt = now.AddMinutes(1),
                    UpdatedAt = now.AddMinutes(2),
                    CompletedAt = now.AddMinutes(2)
                });

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
