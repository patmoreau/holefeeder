using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core.GetNewTransactionOptions;

internal sealed class GetNewTransactionOptionsUseCase(ITransactions transactions)
{
    public async Task<Result<NewTransactionOptions>> HandleAsync(GetNewTransactionOptionsRequest request, CancellationToken cancellationToken)
    {
        var accounts = await transactions.GetOpenAccountsAsync(request.User, cancellationToken);
        if (accounts.IsFailure)
        {
            return accounts.Error;
        }

        var categories = await transactions.GetActiveCategoriesAsync(request.User, cancellationToken);
        if (categories.IsFailure)
        {
            return categories.Error;
        }

        return new NewTransactionOptions(
            [.. accounts.Value.OrderByDescending(account => account.Favorite).ThenBy(account => account.Name, StringComparer.OrdinalIgnoreCase)],
            [.. categories.Value.OrderByDescending(category => category.Favorite).ThenBy(category => category.Name, StringComparer.OrdinalIgnoreCase)]);
    }
}
