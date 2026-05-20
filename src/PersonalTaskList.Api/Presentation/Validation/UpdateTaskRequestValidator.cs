using FluentValidation;
using PersonalTaskList.Api.Presentation.Contracts;

namespace PersonalTaskList.Api.Presentation.Validation;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");
    }
}
