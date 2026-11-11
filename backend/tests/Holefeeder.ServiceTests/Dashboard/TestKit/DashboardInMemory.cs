using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;
using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Dashboard.TestKit;

internal sealed class DashboardInMemory : IDashboard
{
    private readonly Dictionary<Id, PeriodSettings> _settings = [];
    private readonly List<(Id UserId, DailyTotal Total)> _totals = [];
    private readonly List<(Id UserId, Cashflow Cashflow)> _cashflows = [];
    private readonly List<(Id UserId, AccountTotals Account)> _accounts = [];
    private readonly List<(Id UserId, RecentTransaction Transaction)> _transactions = [];
    private readonly List<(Id UserId, UpcomingCashflow Cashflow)> _upcomingCashflows = [];
    private ResultError? _failure;

    public Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<PeriodSettings>>(_failure);
        }

        return Task.FromResult<Result<PeriodSettings>>(
            _settings.TryGetValue(user.Id, out var settings) ? settings : PeriodSettings.Default(today));
    }

    public Task<Result<IReadOnlyList<DailyTotal>>> GetDailyTotalsAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<DailyTotal>>>(_failure);
        }

        IReadOnlyList<DailyTotal> totals = [.. _totals.Where(entry => entry.UserId == user.Id).Select(entry => entry.Total)];
        return Task.FromResult<Result<IReadOnlyList<DailyTotal>>>(totals.ToList());
    }

    public Task<Result<IReadOnlyList<AccountTotals>>> GetActiveAccountsAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<AccountTotals>>>(_failure);
        }

        IReadOnlyList<AccountTotals> accounts = [.. _accounts.Where(entry => entry.UserId == user.Id).Select(entry => entry.Account)];
        return Task.FromResult<Result<IReadOnlyList<AccountTotals>>>(accounts.ToList());
    }

    public Task<Result<IReadOnlyList<Cashflow>>> GetActiveCashflowsAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<Cashflow>>>(_failure);
        }

        IReadOnlyList<Cashflow> cashflows = [.. _cashflows.Where(entry => entry.UserId == user.Id).Select(entry => entry.Cashflow)];
        return Task.FromResult<Result<IReadOnlyList<Cashflow>>>(cashflows.ToList());
    }

    public void Add(User user, PeriodSettings settings) => _settings[user.Id] = settings;

    public void Add(User user, DailyTotal total) => _totals.Add((user.Id, total));

    public void Add(User user, Cashflow cashflow) => _cashflows.Add((user.Id, cashflow));

    public Task<Result<IReadOnlyList<RecentTransaction>>> GetRecentTransactionsAsync(User user, int count, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<RecentTransaction>>>(_failure);
        }

        IReadOnlyList<RecentTransaction> transactions =
        [
            .. _transactions
                .Where(entry => entry.UserId == user.Id)
                .Select(entry => entry.Transaction)
                .OrderByDescending(transaction => transaction.Date)
                .Take(count)
        ];
        return Task.FromResult<Result<IReadOnlyList<RecentTransaction>>>(transactions.ToList());
    }

    public Task<Result<IReadOnlyList<UpcomingCashflow>>> GetUpcomingCashflowsAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<UpcomingCashflow>>>(_failure);
        }

        IReadOnlyList<UpcomingCashflow> cashflows = [.. _upcomingCashflows.Where(entry => entry.UserId == user.Id).Select(entry => entry.Cashflow)];
        return Task.FromResult<Result<IReadOnlyList<UpcomingCashflow>>>(cashflows.ToList());
    }

    public void Add(User user, UpcomingCashflow cashflow) => _upcomingCashflows.Add((user.Id, cashflow));

    public void Add(User user, RecentTransaction transaction) => _transactions.Add((user.Id, transaction));

    public void Add(User user, AccountTotals account) => _accounts.Add((user.Id, account));

    public void FailWith(ResultError error) => _failure = error;
}
