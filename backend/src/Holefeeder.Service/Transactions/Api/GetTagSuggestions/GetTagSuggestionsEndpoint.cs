using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.GetTagSuggestions;

namespace Holefeeder.Service.Transactions.Api.GetTagSuggestions;

internal static class GetTagSuggestionsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetTagSuggestions(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/transactions/tag-suggestions", HandlerAsync)
            .Produces<IReadOnlyList<string>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("TransactionsGetTagSuggestions")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetTagSuggestionsRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        GetTagSuggestionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = GetTagSuggestionsRequestRaw.ToGetTagSuggestionsRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var tagsResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (tagsResult.IsFailure)
        {
            return ApiResult.ToProblem(tagsResult.Error);
        }

        return Results.Ok(tagsResult.Value);
    }
}
