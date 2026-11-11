using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Accounts.Core.CloseAccount;

internal sealed class CloseAccountUseCase(IAccounts accounts)
{
    public async Task<Result<Nothing>> HandleAsync(CloseAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAsync(request.User, request.AccountId, cancellationToken);
        if (account.IsFailure)
        {
            return account.Error;
        }

        var activeCashflows = await accounts.GetActiveCashflowsAsync(request.User, request.AccountId, cancellationToken);
        if (activeCashflows.IsFailure)
        {
            return activeCashflows.Error;
        }

        var closed = account.Value.Close(activeCashflows.Value);
        if (closed.IsFailure)
        {
            return closed.Error;
        }

        return await accounts.SaveAsync(request.User, closed.Value, cancellationToken);
    }
}
