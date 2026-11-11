using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.RecordTransfer;

namespace Holefeeder.Service.Transactions.Api.RecordTransfer;

internal static class RecordTransferEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapRecordTransfer(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/record-transfer", HandlerAsync)
            .Produces<RecordedTransferResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("TransactionsRecordTransfer")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] RecordTransferRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        RecordTransferUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = RecordTransferRequestRaw.ToRecordTransferRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var transferResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (transferResult.IsFailure)
        {
            return ApiResult.ToProblem(transferResult.Error);
        }
        var transfer = transferResult.Value;

        return Results.Created((string?)null, new RecordedTransferResponse(transfer.FromTransactionId.Value, transfer.ToTransactionId.Value));
    }
}
