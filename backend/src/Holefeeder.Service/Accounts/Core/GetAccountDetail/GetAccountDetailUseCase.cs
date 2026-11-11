using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core.GetAccountDetail;

internal sealed class GetAccountDetailUseCase(IAccounts accounts)
{
    public async Task<Result<AccountDetail>> HandleAsync(GetAccountDetailRequest request, CancellationToken cancellationToken)
    {
        var accountResult = await accounts.GetAccountTotalsAsync(request.User, request.AccountId, cancellationToken);
        if (accountResult.IsFailure)
        {
            return accountResult.Error;
        }

        var settingsResult = await accounts.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var cashflowsResult = await accounts.GetActiveCashflowsAsync(request.User, request.AccountId, cancellationToken);
        if (cashflowsResult.IsFailure)
        {
            return cashflowsResult.Error;
        }

        var account = accountResult.Value;
        var periodEnd = Period.Containing(request.Today, settingsResult.Value).End;
        var balance = AccountBalance.Of(account.Type, account.OpenBalance, account.Gains, account.Expenses);
        var upcoming = cashflowsResult.Value.Sum(cashflow => cashflow.UpcomingVariation(periodEnd));

        return new AccountDetail(
            account.Id,
            account.Name,
            account.Type,
            account.Description,
            account.OpenBalance,
            account.OpenDate,
            account.Favorite,
            account.Inactive,
            AccountBalance.CaptionOf(account.Type, balance),
            balance,
            upcoming,
            balance + upcoming,
            account.LastTransactionDate ?? account.OpenDate);
    }
}
