using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Presentation.Contracts;
using PersonalTaskList.Api.Infrastructure.Persistence;

namespace PersonalTaskList.Api.Tests;

public class TaskCreationEndpointTests
{
    [Fact]
    public async Task PostTasks_WithValidData_ReturnsCreatedTaskAndPersistsIt()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Buy groceries",
            description = "Milk, eggs, bread"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/tasks", response.Headers.Location?.OriginalString);

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
        Assert.Equal("Buy groceries", body.Title);
        Assert.Equal("Milk, eggs, bread", body.Description);
        Assert.False(body.IsCompleted);
        Assert.NotEqual(default, body.CreatedAt);
        Assert.NotEqual(default, body.UpdatedAt);
        Assert.Null(body.CompletedAt);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        var savedTask = await dbContext.Tasks.SingleAsync();

        Assert.Equal(body.Id, savedTask.Id);
        Assert.Equal("Buy groceries", savedTask.Title);
    }

    [Fact]
    public async Task PostTasks_WithoutTitle_ReturnsBadRequestAndDoesNotCreateTask()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            description = "Missing title"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        Assert.False(await dbContext.Tasks.AnyAsync());
    }

    [Fact]
    public async Task PostTasks_WithBlankTitle_ReturnsBadRequestAndDoesNotCreateTask()
    {
        using var factory = new TaskApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "   ",
            description = "Blank title"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        Assert.False(await dbContext.Tasks.AnyAsync());
    }
}
