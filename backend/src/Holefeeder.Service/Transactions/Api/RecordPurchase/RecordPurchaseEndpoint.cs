using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.RecordPurchase;

namespace Holefeeder.Service.Transactions.Api.RecordPurchase;

internal static class RecordPurchaseEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapRecordPurchase(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/record-purchase", HandlerAsync)
            .Produces<TransactionResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("TransactionsRecordPurchase")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] RecordPurchaseRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        RecordPurchaseUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = RecordPurchaseRequestRaw.ToRecordPurchaseRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var idResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }

        return Results.Created((string?)null, new TransactionResponse(idResult.Value.Value));
    }
}
