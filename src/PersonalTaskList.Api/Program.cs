using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => Results.Ok("Personal Task List API"));

app.Run();
