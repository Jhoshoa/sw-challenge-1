using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalTaskList.Api.Contracts;
using PersonalTaskList.Api.Data;

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

internal sealed class TaskApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<TaskDbContext>));

            services.Remove(dbContextDescriptor);

            services.AddDbContext<TaskDbContext>(options => options.UseSqlite(_connection));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
