using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PersonalTaskList.Api.Application.Tasks;
using PersonalTaskList.Api.Domain.Tasks;
using PersonalTaskList.Api.Infrastructure.Persistence;
using PersonalTaskList.Api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<TaskService>();

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program;
