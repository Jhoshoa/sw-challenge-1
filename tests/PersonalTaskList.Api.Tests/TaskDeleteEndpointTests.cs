using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Domain.Tasks;
using PersonalTaskList.Api.Infrastructure.Persistence;

namespace PersonalTaskList.Api.Tests;

public class TaskDeleteEndpointTests
{
    [Fact]
    public async Task DeleteTask_WhenTaskExists_ReturnsNoContentAndRemovesTask()
    {
        using var factory = new TaskApiFactory();
        var taskId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Tasks.Add(TaskItem.Create(taskId, "Delete me", null, now));

            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/tasks/{taskId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<TaskDbContext>();

        Assert.False(await verifyDbContext.Tasks.AnyAsync(task => task.Id == taskId));
    }

    [Fact]
    public async Task DeleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
