namespace PersonalTaskList.Api.Presentation.Contracts;

public sealed record CreateTaskRequest(string? Title, string? Description);
