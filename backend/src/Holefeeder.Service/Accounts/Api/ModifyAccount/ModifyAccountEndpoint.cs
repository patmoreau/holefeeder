using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.ModifyAccount;

internal static class ModifyAccountEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapModifyAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/modify-account", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("ModifyAccount")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ModifyAccountRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ModifyAccountUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ModifyAccountRequestRaw.ToModifyAccountRequest(rawRequest, userContextAccessor.Current);
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
