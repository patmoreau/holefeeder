using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

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
        var title = FlowTitle.Of(transaction.Description, transaction.CategoryName);

        return new DashboardTransaction(
            transaction.Id,
            title.Title,
            title.Category,
            transaction.CategoryColor,
            transaction.Tags,
            transaction.Amount,
            transaction.Date);
    }
}
