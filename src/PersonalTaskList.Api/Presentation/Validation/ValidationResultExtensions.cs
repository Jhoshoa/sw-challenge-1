using FluentValidation.Results;

namespace PersonalTaskList.Api.Presentation.Validation;

public static class ValidationResultExtensions
{
    public static Dictionary<string, string[]> ToValidationProblemErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
    }
}
