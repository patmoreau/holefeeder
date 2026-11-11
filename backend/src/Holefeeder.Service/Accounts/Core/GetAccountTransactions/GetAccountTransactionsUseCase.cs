using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.Service.Accounts.Core.GetAccountTransactions;

internal sealed class GetAccountTransactionsUseCase(IAccounts accounts)
{
    public async Task<Result<PagedData<AccountTransaction>>> HandleAsync(GetAccountTransactionsRequest request, CancellationToken cancellationToken)
    {
        var transactionsResult = await accounts.GetTransactionsAsync(request.User, request.AccountId, request.Offset, request.Limit, cancellationToken);
        if (transactionsResult.IsFailure)
        {
            return transactionsResult.Error;
        }
        var transactions = transactionsResult.Value;

        return new PagedData<AccountTransaction>(transactions.Total, [.. transactions.Items.Select(ToAccountTransaction)]);
    }

    private static AccountTransaction ToAccountTransaction(RecordedTransaction transaction)
    {
        var title = FlowTitle.Of(transaction.Description, transaction.CategoryName);

        return new AccountTransaction(
            transaction.Id,
            title.Title,
            title.Category,
            transaction.CategoryColor,
            transaction.Tags,
            transaction.Amount,
            transaction.Date);
    }
}
