using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core;

internal interface IAccounts
{
    Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken);

    Task<Result<AccountTotals>> GetAccountTotalsAsync(User user, Id accountId, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<CashflowSchedule>>> GetActiveCashflowsAsync(User user, Id accountId, CancellationToken cancellationToken);

    Task<Result<PagedData<RecordedTransaction>>> GetTransactionsAsync(User user, Id accountId, Offset offset, Limit limit, CancellationToken cancellationToken);
}
