using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PersonalTaskList.Api.Application.Tasks;
using PersonalTaskList.Api.Presentation.Dtos;
using PersonalTaskList.Api.Presentation.Validation;

namespace PersonalTaskList.Api.Presentation.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> ListAsync(CancellationToken cancellationToken)
    {
        var tasks = await _taskService.ListAsync(cancellationToken);

        return Ok(tasks.Select(TaskResponse.FromTaskEntityToDto));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateAsync(
        CreateTaskRequest request,
        IValidator<CreateTaskRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToValidationProblemErrors()));
        }

        var task = await _taskService.CreateAsync(request.Title!, request.Description, cancellationToken);

        return Created("/api/tasks", TaskResponse.FromTaskEntityToDto(task));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> UpdateAsync(
        Guid id,
        UpdateTaskRequest request,
        IValidator<UpdateTaskRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToValidationProblemErrors()));
        }

        var task = await _taskService.UpdateAsync(id, request.Title!, request.Description, cancellationToken);

        return task is null ? NotFound() : Ok(TaskResponse.FromTaskEntityToDto(task));
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<TaskResponse>> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.CompleteAsync(id, cancellationToken);

        return task is null ? NotFound() : Ok(TaskResponse.FromTaskEntityToDto(task));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _taskService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
