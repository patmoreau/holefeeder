using Holefeeder.Service.Accounts.Core.FavoriteAccount;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.FavoriteAccount;

internal static class FavoriteAccountEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapFavoriteAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/favorite-account", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("FavoriteAccount")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] FavoriteAccountRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        FavoriteAccountUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = FavoriteAccountRequestRaw.ToFavoriteAccountRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var saveResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (saveResult.IsFailure)
        {
            return ApiResult.ToProblem(saveResult.Error);
        }

        return Results.NoContent();
    }
}
