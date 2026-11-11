using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.ServiceTests.Transactions.TestKit;

internal sealed class TransactionsInMemory : ITransactions
{
    private readonly List<(Id UserId, TransactionAccount Account)> _accounts = [];
    private readonly List<(Id UserId, TransactionCategory Category)> _categories = [];
    private ResultError? _failure;

    public void Add(User user, TransactionAccount account) => _accounts.Add((user.Id, account));

    public void Add(User user, TransactionCategory category) => _categories.Add((user.Id, category));

    public void FailWith(ResultError error) => _failure = error;

    public Task<Result<IReadOnlyList<TransactionAccount>>> GetOpenAccountsAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(Of(_accounts.Where(entry => entry.UserId == user.Id).Select(entry => entry.Account).ToList()));

    public Task<Result<IReadOnlyList<TransactionCategory>>> GetActiveCategoriesAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(Of(_categories.Where(entry => entry.UserId == user.Id).Select(entry => entry.Category).ToList()));

    private Result<IReadOnlyList<T>> Of<T>(List<T> items)
    {
        if (_failure is not null)
        {
            return _failure;
        }

        return items;
    }
}
