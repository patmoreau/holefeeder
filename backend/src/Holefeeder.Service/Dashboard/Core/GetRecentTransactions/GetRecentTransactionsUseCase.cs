using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core.GetRecentTransactions;

internal sealed class GetRecentTransactionsUseCase(IDashboard dashboard)
{
    public async Task<Result<IReadOnlyList<DashboardTransaction>>> HandleAsync(GetRecentTransactionsRequest request, CancellationToken cancellationToken)
    {
        var transactionsResult = await dashboard.GetRecentTransactionsAsync(request.User, request.Count, cancellationToken);
        if (transactionsResult.IsFailure)
        {
            return transactionsResult.Error;
        }

        return transactionsResult.Value.Select(ToDashboardTransaction).ToList();
    }

    private static DashboardTransaction ToDashboardTransaction(RecentTransaction transaction)
    {
        var description = transaction.Description.Trim();
        var hasDescription = description.Length > 0;

        return new DashboardTransaction(
            transaction.Id,
            hasDescription ? description : transaction.CategoryName,
            hasDescription ? transaction.CategoryName : null,
            transaction.CategoryColor,
            transaction.Tags,
            transaction.Amount,
            transaction.Date);
    }
}
