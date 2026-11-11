using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core.GetTransaction;

internal sealed class GetTransactionUseCase(ITransactions transactions)
{
    public Task<Result<TransactionDetail>> HandleAsync(GetTransactionRequest request, CancellationToken cancellationToken) =>
        transactions.GetTransactionAsync(request.User, request.Id, cancellationToken);
}
