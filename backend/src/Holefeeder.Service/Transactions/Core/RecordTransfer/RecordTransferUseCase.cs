using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core.RecordTransfer;

internal sealed class RecordTransferUseCase(ITransactions transactions)
{
    public async Task<Result<RecordedTransfer>> HandleAsync(RecordTransferRequest request, CancellationToken cancellationToken)
    {
        if (request.FromAccountId == request.ToAccountId)
        {
            return TransactionErrors.SameAccount;
        }

        var from = await transactions.GetOpenAccountAsync(request.User, request.FromAccountId, cancellationToken);
        if (from.IsFailure)
        {
            return from.Error;
        }

        var to = await transactions.GetOpenAccountAsync(request.User, request.ToAccountId, cancellationToken);
        if (to.IsFailure)
        {
            return to.Error;
        }

        var categories = await transactions.GetTransferCategoriesAsync(request.User, cancellationToken);
        if (categories.IsFailure)
        {
            return categories.Error;
        }

        var description = string.IsNullOrWhiteSpace(request.Description)
            ? $"Transfer from '{from.Value.Name}' to '{to.Value.Name}'"
            : request.Description;
        var withdrawal = new Transaction(Id.New(), request.Date, request.Amount, description, request.FromAccountId,
            categories.Value.TransferOut, TagList.Create([]), null, null);
        var deposit = withdrawal with { Id = Id.New(), AccountId = request.ToAccountId, CategoryId = categories.Value.TransferIn };

        var saved = await transactions.SaveAsync(request.User, [withdrawal, deposit], null, cancellationToken);
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        return new RecordedTransfer(withdrawal.Id, deposit.Id);
    }
}
