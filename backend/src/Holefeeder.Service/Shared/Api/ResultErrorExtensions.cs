using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Api;

internal static class ResultErrorExtensions
{
    internal static IResult ToProblemDetails(this ResultError error, int statusCode) => Results.Problem(statusCode: statusCode, detail: error.Description);

    internal static IResult ToValidationProblemDetails(this ResultErrorAggregate error) =>
        Results.ValidationProblem(error.Errors.ToDictionary(entry => entry.Key, entry => entry.Value),
            error.Description);
}
