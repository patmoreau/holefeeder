using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Dashboard.Api.GetRecentTransactions;

internal static class GetRecentTransactionsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";
    private const int RecentCount = 4;

    public static IEndpointRouteBuilder MapGetRecentTransactions(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/dashboard/transactions", HandlerAsync)
            .Produces<IReadOnlyList<DashboardTransactionResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Dashboard")
            .WithName("GetRecentTransactions")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        GetRecentTransactionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new GetRecentTransactionsRequest(userContextAccessor.Current, RecentCount);

        var transactionsResult = await useCase.HandleAsync(request, cancellationToken);
        if (transactionsResult.IsFailure)
        {
            return ApiResult.ToProblem(transactionsResult.Error);
        }

        return Results.Ok(transactionsResult.Value.Select(transaction => new DashboardTransactionResponse(
            transaction.Id.Value,
            transaction.Title,
            transaction.Category,
            transaction.CategoryColor,
            transaction.Tags,
            transaction.Amount,
            transaction.Date)).ToList());
    }
}
