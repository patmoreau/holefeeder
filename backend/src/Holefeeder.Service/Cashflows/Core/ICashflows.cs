using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Core;

internal interface ICashflows
{
    Task<Result<IReadOnlyList<CashflowSummary>>> ListAsync(User user, CancellationToken cancellationToken);

    Task<Result<Cashflow>> GetAsync(User user, Id id, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(User user, Cashflow cashflow, Payment? payment, CancellationToken cancellationToken);
}
