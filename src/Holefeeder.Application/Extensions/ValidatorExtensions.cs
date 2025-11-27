using DrifterApps.Seeds.FluentResult;

using FluentValidation.Results;

namespace Holefeeder.Application.Extensions;

public static class ValidatorExtensions
{
    public const string CodeValidationErrors = "FluentValidation.ValidationErrors";

    internal static async Task<Result<T>> IsValidAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken)
    {
        if (instance is null)
        {
            return new ResultError(CodeValidationErrors, $"Request '{typeof(T).FullName}' is missing.");
        }

        var validation = await validator.ValidateAsync(instance, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationErrors(typeof(T), validation.Errors);
        }

        return instance;
    }

    private static ResultErrorAggregate ValidationErrors(Type requestType,
        IReadOnlyList<ValidationFailure> validationFailures) =>
        new(
            CodeValidationErrors,
            $"Validation errors were found for request '{requestType.FullName}'.",
            validationFailures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));
}
