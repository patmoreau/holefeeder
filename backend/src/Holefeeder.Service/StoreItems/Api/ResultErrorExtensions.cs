using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.StoreItems.Api;

internal static class ResultErrorExtensions
{
    internal static IResult ToProblem(this ResultError error) =>
        error switch
        {
            ResultErrorAggregate validationError => validationError.ToValidationProblemDetails(),
            _ => error.ToProblemDetails(StatusCodes.Status500InternalServerError)
        };
}
