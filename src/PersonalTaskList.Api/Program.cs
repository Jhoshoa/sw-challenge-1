using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Data;
using PersonalTaskList.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/api/tasks", async (TaskDbContext dbContext) =>
{
    var tasks = await dbContext.Tasks
        .AsNoTracking()
        .OrderBy(task => task.Id)
        .Select(task => TaskResponse.FromTask(task))
        .ToListAsync();

    return Results.Ok(tasks);
});

app.MapPost("/api/tasks", async (CreateTaskRequest request, TaskDbContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = ["Title is required."]
        });
    }

    var now = DateTimeOffset.UtcNow;
    var task = new PersonalTaskList.Api.Models.Task
    {
        Id = Guid.NewGuid(),
        Title = request.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
        IsCompleted = false,
        CreatedAt = now,
        UpdatedAt = now,
        CompletedAt = null
    };

    dbContext.Tasks.Add(task);
    await dbContext.SaveChangesAsync();

    return Results.Created("/api/tasks", TaskResponse.FromTask(task));
});

app.MapPut("/api/tasks/{id:guid}", async (Guid id, UpdateTaskRequest request, TaskDbContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = ["Title is required."]
        });
    }

    var task = await dbContext.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.NotFound();
    }

    task.Title = request.Title.Trim();
    task.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description;
    task.UpdatedAt = DateTimeOffset.UtcNow;

    await dbContext.SaveChangesAsync();

    return Results.Ok(TaskResponse.FromTask(task));
});

app.MapPatch("/api/tasks/{id:guid}/complete", async (Guid id, TaskDbContext dbContext) =>
{
    var task = await dbContext.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.NotFound();
    }

    if (!task.IsCompleted)
    {
        var now = DateTimeOffset.UtcNow;
        task.IsCompleted = true;
        task.CompletedAt = now;
        task.UpdatedAt = now;

        await dbContext.SaveChangesAsync();
    }

    return Results.Ok(TaskResponse.FromTask(task));
});

app.MapDelete("/api/tasks/{id:guid}", async (Guid id, TaskDbContext dbContext) =>
{
    var task = await dbContext.Tasks.FindAsync(id);

    if (task is null)
    {
        return Results.NotFound();
    }

    dbContext.Tasks.Remove(task);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

public partial class Program;
