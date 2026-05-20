using FluentValidation;
using PersonalTaskList.Api.Presentation.Dtos;

namespace PersonalTaskList.Api.Presentation.Validation;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Title is required.");
    }
}
