using Holefeeder.Service.Accounts.Core.CloseAccount;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.CloseAccount;

internal static class CloseAccountEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapCloseAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/close-account", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("CloseAccount")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] CloseAccountRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        CloseAccountUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = CloseAccountRequestRaw.ToCloseAccountRequest(rawRequest, userContextAccessor.Current);
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
