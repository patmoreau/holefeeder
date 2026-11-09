using System.Globalization;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Shared.Api;

internal static class ApiResult
{
    public static IResult ToPagedOk<T>(int total, IEnumerable<T> items, HttpContext context)
    {
        context.Response.Headers.Append("X-Total-Count", total.ToString(CultureInfo.InvariantCulture));
        return Results.Ok(items);
    }

    public static IResult ToProblem(ResultError error) =>
        error switch
        {
            ResultErrorAggregate validationError => ToValidationProblemDetails(validationError),
            UnexpectedError unexpectedError => ToProblemDetails(unexpectedError, StatusCodes.Status500InternalServerError),
            NotFoundError notFoundError => ToProblemDetails(notFoundError, StatusCodes.Status404NotFound),
            _ => ToProblemDetails(error, StatusCodes.Status400BadRequest)
        };

    private static IResult ToProblemDetails(ResultError error, int statusCode) => Results.Problem(statusCode: statusCode, detail: error.Description);

    private static IResult ToValidationProblemDetails(ResultErrorAggregate error) =>
        Results.ValidationProblem(error.Errors.ToDictionary(entry => entry.Key, entry => entry.Value), error.Description);
}
