using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.GetAccountTransactions;

internal static class GetAccountTransactionsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetAccountTransactions(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/accounts/{id:guid}/transactions", HandlerAsync)
            .Produces<IReadOnlyList<AccountTransactionResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("GetAccountTransactions")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetAccountTransactionsRequestRaw rawRequest,
        HttpContext context,
        UserContextAccessor userContextAccessor,
        GetAccountTransactionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = GetAccountTransactionsRequestRaw.ToGetAccountTransactionsRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }
        var request = requestResult.Value;

        var pagedResult = await useCase.HandleAsync(request, cancellationToken);
        if (pagedResult.IsFailure)
        {
            return ApiResult.ToProblem(pagedResult.Error);
        }
        var paged = pagedResult.Value;

        return ApiResult.ToPagedOk(paged.Total, paged.Items.Select(transaction => new AccountTransactionResponse(
            transaction.Id.Value,
            transaction.Title,
            transaction.Category,
            transaction.CategoryColor,
            transaction.Tags,
            transaction.Amount,
            transaction.Date)), context);
    }
}
