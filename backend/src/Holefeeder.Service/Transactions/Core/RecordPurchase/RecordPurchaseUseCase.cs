using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core.RecordPurchase;

internal sealed class RecordPurchaseUseCase(ITransactions transactions)
{
    public async Task<Result<Id>> HandleAsync(RecordPurchaseRequest request, CancellationToken cancellationToken)
    {
        var account = await transactions.GetOpenAccountAsync(request.User, request.AccountId, cancellationToken);
        if (account.IsFailure)
        {
            return account.Error;
        }

        var category = await transactions.GetActiveCategoryAsync(request.User, request.CategoryId, cancellationToken);
        if (category.IsFailure)
        {
            return category.Error;
        }

        var cashflow = request.Repeat is { } repeat
            ? new Cashflow(Id.New(), repeat.StartDate, request.Amount, repeat.IntervalType, repeat.Frequency, request.Description,
                request.AccountId, request.CategoryId, request.Tags)
            : null;
        var transaction = new Transaction(Id.New(), request.Date, request.Amount, request.Description, request.AccountId,
            request.CategoryId, request.Tags, cashflow?.Id, cashflow is null ? null : request.Date);

        var saved = await transactions.SaveAsync(request.User, [transaction], cashflow, cancellationToken);
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        return transaction.Id;
    }
}
