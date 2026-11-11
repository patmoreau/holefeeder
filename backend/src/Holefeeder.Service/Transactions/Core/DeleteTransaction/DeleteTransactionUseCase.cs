using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core.DeleteTransaction;

internal sealed class DeleteTransactionUseCase(ITransactions transactions)
{
    public Task<Result<Nothing>> HandleAsync(DeleteTransactionRequest request, CancellationToken cancellationToken) =>
        transactions.DeleteAsync(request.User, request.Id, cancellationToken);
}
