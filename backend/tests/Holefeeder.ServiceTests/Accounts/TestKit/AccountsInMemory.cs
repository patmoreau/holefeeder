using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Accounts.TestKit;

internal sealed class AccountsInMemory : IAccounts
{
    private readonly Dictionary<Id, PeriodSettings> _settings = [];
    private readonly List<(Id UserId, AccountTotals Account)> _accounts = [];
    private readonly List<(Id UserId, CashflowSchedule Cashflow)> _cashflows = [];
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

    public void Add(User user, PeriodSettings settings) => _settings[user.Id] = settings;

    public void Add(User user, AccountTotals account) => _accounts.Add((user.Id, account));

    public void Add(User user, CashflowSchedule cashflow) => _cashflows.Add((user.Id, cashflow));

    public void FailWith(ResultError error) => _failure = error;
}
