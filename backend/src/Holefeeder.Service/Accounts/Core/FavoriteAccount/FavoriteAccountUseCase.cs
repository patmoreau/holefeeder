using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Accounts.Core.FavoriteAccount;

internal sealed class FavoriteAccountUseCase(IAccounts accounts)
{
    public async Task<Result<Nothing>> HandleAsync(FavoriteAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAsync(request.User, request.AccountId, cancellationToken);
        if (account.IsFailure)
        {
            return account.Error;
        }

        return await accounts.SaveAsync(request.User, account.Value.SetFavorite(request.IsFavorite), cancellationToken);
    }
}
