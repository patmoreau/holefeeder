using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;

internal sealed class GetDashboardAccountsUseCase(IDashboard dashboard)
{
    public async Task<Result<IReadOnlyList<DashboardAccount>>> HandleAsync(GetDashboardAccountsRequest request, CancellationToken cancellationToken)
    {
        var settingsResult = await dashboard.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var accountsResult = await dashboard.GetActiveAccountsAsync(request.User, cancellationToken);
        if (accountsResult.IsFailure)
        {
            return accountsResult.Error;
        }

        var cashflowsResult = await dashboard.GetActiveCashflowsAsync(request.User, cancellationToken);
        if (cashflowsResult.IsFailure)
        {
            return cashflowsResult.Error;
        }

        var periodEnd = Period.Containing(request.Today, settingsResult.Value).End;

        return accountsResult.Value
            .Select(account => ToDashboardAccount(account, cashflowsResult.Value.Where(cashflow => cashflow.AccountId == account.Id), periodEnd))
            .ToList();
    }

    private static DashboardAccount ToDashboardAccount(AccountTotals account, IEnumerable<Cashflow> cashflows, DateOnly periodEnd)
    {
        var balance = account.Type.Sign * account.OpenBalance + account.Gains - account.Expenses;
        var projectedBalance = balance + cashflows.Sum(cashflow => cashflow.UpcomingVariation(periodEnd));

        return new DashboardAccount(
            account.Id,
            account.Name,
            CaptionOf(account.Type, balance),
            balance,
            projectedBalance,
            account.LastTransactionDate ?? account.OpenDate);
    }

    private static BalanceCaption CaptionOf(AccountType type, decimal balance) =>
        Math.Round(balance, 2) switch
        {
            < 0 => BalanceCaption.AmountDue,
            > 0 => type.IsAsset ? BalanceCaption.AvailableBalance : BalanceCaption.CreditBalance,
            _ => type.IsAsset ? BalanceCaption.AvailableBalance : BalanceCaption.AmountDue
        };
}
