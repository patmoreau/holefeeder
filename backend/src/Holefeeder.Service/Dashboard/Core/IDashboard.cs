using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;
using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Core;

internal interface IDashboard
{
    Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<DailyTotal>>> GetDailyTotalsAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AccountTotals>>> GetActiveAccountsAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<CashflowSchedule>>> GetActiveCashflowsAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<RecentTransaction>>> GetRecentTransactionsAsync(User user, int count, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<UpcomingCashflow>>> GetUpcomingCashflowsAsync(User user, CancellationToken cancellationToken);
}
