using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.DeleteTransaction;

namespace Holefeeder.Service.Transactions.Api.DeleteTransaction;

internal static class DeleteTransactionEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapDeleteTransaction(this IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v2/transactions/{id:guid}", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("DeleteTransaction")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] DeleteTransactionRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        DeleteTransactionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = DeleteTransactionRequestRaw.ToDeleteTransactionRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var deleteResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return ApiResult.ToProblem(deleteResult.Error);
        }

        return Results.NoContent();
    }
}
