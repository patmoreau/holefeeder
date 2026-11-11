using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Accounts.Core.ModifyAccount;

internal sealed class ModifyAccountUseCase(IAccounts accounts)
{
    public async Task<Result<Nothing>> HandleAsync(ModifyAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAsync(request.User, request.AccountId, cancellationToken);
        if (account.IsFailure)
        {
            return account.Error;
        }

        return await accounts.SaveAsync(
            request.User, account.Value.Modify(request.Name, request.Type, request.OpenDate, request.OpenBalance, request.Description), cancellationToken);
    }
}
