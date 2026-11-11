using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Accounts.TestKit;

internal sealed class AccountsInMemory : IAccounts
{
    private readonly Dictionary<Id, PeriodSettings> _settings = [];
    private readonly List<(Id UserId, AccountTotals Account)> _accounts = [];
    private readonly List<(Id UserId, CashflowSchedule Cashflow)> _cashflows = [];
    private readonly List<(Id UserId, Id AccountId)> _accountIds = [];
    private readonly List<RecordedTransaction> _transactions = [];
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

    public Task<Result<AccountTotals>> GetAccountTotalsAsync(User user, Id accountId, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<AccountTotals>>(_failure);
        }

        var account = _accounts.Where(entry => entry.UserId == user.Id && entry.Account.Id == accountId).Select(entry => entry.Account).FirstOrDefault();
        return Task.FromResult<Result<AccountTotals>>(account is null ? AccountErrors.NotFound : account);
    }

    public Task<Result<IReadOnlyList<CashflowSchedule>>> GetActiveCashflowsAsync(User user, Id accountId, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<IReadOnlyList<CashflowSchedule>>>(_failure);
        }

        IReadOnlyList<CashflowSchedule> cashflows =
            [.. _cashflows.Where(entry => entry.UserId == user.Id && entry.Cashflow.AccountId == accountId).Select(entry => entry.Cashflow)];
        return Task.FromResult<Result<IReadOnlyList<CashflowSchedule>>>(cashflows.ToList());
    }

    public Task<Result<PagedData<RecordedTransaction>>> GetTransactionsAsync(
        User user, Id accountId, Offset offset, Limit limit, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<PagedData<RecordedTransaction>>>(_failure);
        }

        var owned = _accountIds.Contains((user.Id, accountId)) || _accounts.Any(entry => entry.UserId == user.Id && entry.Account.Id == accountId);
        if (!owned)
        {
            return Task.FromResult<Result<PagedData<RecordedTransaction>>>(AccountErrors.NotFound);
        }

        var transactions = _transactions
            .Where(transaction => transaction.AccountId == accountId)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.Id.Value)
            .ToList();
        return Task.FromResult<Result<PagedData<RecordedTransaction>>>(
            new PagedData<RecordedTransaction>(transactions.Count, [.. transactions.Skip(offset.Value).Take(limit.Value)]));
    }

    public Task<Result<Account>> GetAsync(User user, Id accountId, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Account>>(_failure);
        }

        var account = _accounts.Where(entry => entry.UserId == user.Id && entry.Account.Id == accountId).Select(entry => entry.Account).FirstOrDefault();
        return Task.FromResult<Result<Account>>(account is null
            ? AccountErrors.NotFound
            : new Account(account.Id, account.Type, AccountName.Valid(account.Name), account.OpenDate, Money.Valid(account.OpenBalance),
                account.Description, account.Favorite, account.Inactive));
    }

    public Task<Result<Nothing>> SaveAsync(User user, Account account, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Nothing>>(_failure);
        }

        if (_accounts.Exists(entry => entry.UserId == user.Id && entry.Account.Name == account.Name.Value && entry.Account.Id != account.Id))
        {
            return Task.FromResult<Result<Nothing>>(AccountNameErrors.AlreadyExists);
        }

        var index = _accounts.FindIndex(entry => entry.Account.Id == account.Id);
        if (index < 0)
        {
            _accounts.Add((user.Id, ToAccountTotals(account, null)));
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }

        if (_accounts[index].UserId != user.Id)
        {
            return Task.FromResult<Result<Nothing>>(AccountErrors.NotFound);
        }

        _accounts[index] = (user.Id, ToAccountTotals(account, _accounts[index].Account));
        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public void Add(User user, Id accountId, params RecordedTransaction[] transactions)
    {
        _accountIds.Add((user.Id, accountId));
        _transactions.AddRange(transactions);
    }

    public void Add(User user, PeriodSettings settings) => _settings[user.Id] = settings;

    public void Add(User user, AccountTotals account) => _accounts.Add((user.Id, account));

    public void Add(User user, CashflowSchedule cashflow) => _cashflows.Add((user.Id, cashflow));

    public void FailWith(ResultError error) => _failure = error;

    private static AccountTotals ToAccountTotals(Account account, AccountTotals? totals) =>
        new(account.Id, account.Name.Value, account.Type, account.Description, account.OpenBalance.Value, account.OpenDate,
            account.Favorite, account.Inactive, totals?.Gains ?? 0m, totals?.Expenses ?? 0m, totals?.LastTransactionDate);
}
