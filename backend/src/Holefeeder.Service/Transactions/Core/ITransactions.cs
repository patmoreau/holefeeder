using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core;

internal interface ITransactions
{
    Task<Result<IReadOnlyList<TransactionAccount>>> GetOpenAccountsAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<TransactionCategory>>> GetActiveCategoriesAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<TagUsage>>> GetTagUsagesAsync(User user, CancellationToken cancellationToken);
}
