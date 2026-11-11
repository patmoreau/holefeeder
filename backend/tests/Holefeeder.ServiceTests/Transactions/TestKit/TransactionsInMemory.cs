using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.ServiceTests.Transactions.TestKit;

internal sealed class TransactionsInMemory : ITransactions
{
    private readonly List<(Id UserId, TransactionAccount Account)> _accounts = [];
    private readonly List<(Id UserId, TransactionCategory Category)> _categories = [];
    private readonly List<(Id UserId, TagUsage Usage)> _tagUsages = [];
    private readonly List<(Id UserId, Transaction Transaction)> _transactions = [];
    private readonly List<(Id UserId, Cashflow Cashflow)> _cashflows = [];
    private readonly Dictionary<Id, TransferCategories> _transferCategories = [];
    private ResultError? _failure;

    public void Add(User user, TransactionAccount account) => _accounts.Add((user.Id, account));

    public void Add(User user, TransactionCategory category) => _categories.Add((user.Id, category));

    public void Add(User user, TagUsage usage) => _tagUsages.Add((user.Id, usage));

    public IReadOnlyList<Transaction> TransactionsOf(User user) =>
        [.. _transactions.Where(entry => entry.UserId == user.Id).Select(entry => entry.Transaction)];

    public IReadOnlyList<Cashflow> CashflowsOf(User user) =>
        [.. _cashflows.Where(entry => entry.UserId == user.Id).Select(entry => entry.Cashflow)];

    public void Add(User user, TransferCategories categories) => _transferCategories[user.Id] = categories;

    public void FailWith(ResultError error) => _failure = error;

    public Task<Result<IReadOnlyList<TransactionAccount>>> GetOpenAccountsAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(Of(_accounts.Where(entry => entry.UserId == user.Id).Select(entry => entry.Account).ToList()));

    public Task<Result<IReadOnlyList<TransactionCategory>>> GetActiveCategoriesAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(Of(_categories.Where(entry => entry.UserId == user.Id).Select(entry => entry.Category).ToList()));

    public Task<Result<IReadOnlyList<TagUsage>>> GetTagUsagesAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(Of(_tagUsages.Where(entry => entry.UserId == user.Id).Select(entry => entry.Usage).ToList()));

    public Task<Result<TransactionAccount>> GetOpenAccountAsync(User user, Id id, CancellationToken cancellationToken) =>
        Task.FromResult(One(_accounts.Where(entry => entry.UserId == user.Id && entry.Account.Id == id).Select(entry => entry.Account), AccountErrors.NotFound));

    public Task<Result<TransactionCategory>> GetActiveCategoryAsync(User user, Id id, CancellationToken cancellationToken) =>
        Task.FromResult(One(_categories.Where(entry => entry.UserId == user.Id && entry.Category.Id == id).Select(entry => entry.Category), TransactionErrors.CategoryNotFound));

    public Task<Result<TransferCategories>> GetTransferCategoriesAsync(User user, CancellationToken cancellationToken) =>
        Task.FromResult(One(_transferCategories.TryGetValue(user.Id, out var categories) ? [categories] : Array.Empty<TransferCategories>(),
            TransactionErrors.TransferCategoriesNotFound));

    public Task<Result<Nothing>> SaveAsync(User user, IReadOnlyList<Transaction> transactions, Cashflow? cashflow, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        if (cashflow is not null)
        {
            _cashflows.Add((user.Id, cashflow));
        }
        _transactions.AddRange(transactions.Select(transaction => (user.Id, transaction)));
        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    private Result<T> One<T>(IEnumerable<T> items, ResultError notFound)
    {
        if (_failure is not null)
        {
            return _failure;
        }

        var item = items.SingleOrDefault();
        return item is null ? notFound : item;
    }

    private Result<IReadOnlyList<T>> Of<T>(List<T> items)
    {
        if (_failure is not null)
        {
            return _failure;
        }

        return items;
    }
}
