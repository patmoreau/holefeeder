using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core;

internal interface ITransactions
{
    Task<Result<IReadOnlyList<TransactionAccount>>> GetOpenAccountsAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<TransactionCategory>>> GetActiveCategoriesAsync(User user, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<TagUsage>>> GetTagUsagesAsync(User user, CancellationToken cancellationToken);

    Task<Result<TransactionAccount>> GetOpenAccountAsync(User user, Id id, CancellationToken cancellationToken);

    Task<Result<TransactionCategory>> GetActiveCategoryAsync(User user, Id id, CancellationToken cancellationToken);

    Task<Result<TransferCategories>> GetTransferCategoriesAsync(User user, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(User user, IReadOnlyList<Transaction> transactions, Cashflow? cashflow, CancellationToken cancellationToken);
}
