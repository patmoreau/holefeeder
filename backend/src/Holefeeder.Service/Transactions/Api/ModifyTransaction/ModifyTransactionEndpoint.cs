using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.ModifyTransaction;

namespace Holefeeder.Service.Transactions.Api.ModifyTransaction;

internal static class ModifyTransactionEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapModifyTransaction(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/modify", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("ModifyTransaction")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ModifyTransactionRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ModifyTransactionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ModifyTransactionRequestRaw.ToModifyTransactionRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var modifyResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (modifyResult.IsFailure)
        {
            return ApiResult.ToProblem(modifyResult.Error);
        }

        return Results.NoContent();
    }
}
