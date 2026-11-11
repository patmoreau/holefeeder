using System.Text.Json;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.GetTransaction;

namespace Holefeeder.Service.Transactions.Api.GetTransaction;

internal static class GetTransactionEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetTransaction(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/transactions/{id:guid}", HandlerAsync)
            .Produces<TransactionDetailResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Transactions")
            .WithName("GetTransaction")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetTransactionRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        GetTransactionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = GetTransactionRequestRaw.ToGetTransactionRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var transactionResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (transactionResult.IsFailure)
        {
            return ApiResult.ToProblem(transactionResult.Error);
        }
        var transaction = transactionResult.Value;

        return Results.Ok(new TransactionDetailResponse(
            transaction.Id.Value,
            transaction.Date,
            transaction.Amount.Value,
            transaction.Description,
            transaction.Tags.Values,
            new TransactionDetailCategoryResponse(
                transaction.Category.Id.Value,
                transaction.Category.Name,
                JsonNamingPolicy.CamelCase.ConvertName(transaction.Category.Type.ToString()),
                transaction.Category.Color),
            new TransactionDetailAccountResponse(transaction.Account.Id.Value, transaction.Account.Name)));
    }
}
