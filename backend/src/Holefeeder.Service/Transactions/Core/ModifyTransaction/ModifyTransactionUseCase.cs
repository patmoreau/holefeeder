using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core.ModifyTransaction;

internal sealed class ModifyTransactionUseCase(ITransactions transactions)
{
    public async Task<Result<Nothing>> HandleAsync(ModifyTransactionRequest request, CancellationToken cancellationToken)
    {
        var hasAccount = await transactions.HasAccountAsync(request.User, request.AccountId, cancellationToken);
        if (hasAccount.IsFailure)
        {
            return hasAccount.Error;
        }
        if (!hasAccount.Value)
        {
            return TransactionErrors.AccountMissing(request.AccountId);
        }

        var hasCategory = await transactions.HasCategoryAsync(request.User, request.CategoryId, cancellationToken);
        if (hasCategory.IsFailure)
        {
            return hasCategory.Error;
        }
        if (!hasCategory.Value)
        {
            return TransactionErrors.CategoryMissing(request.CategoryId);
        }

        var existing = await transactions.LoadTransactionAsync(request.User, request.Id, cancellationToken);
        if (existing.IsFailure)
        {
            return existing.Error;
        }

        var modified = existing.Value with
        {
            Date = request.Date,
            Amount = request.Amount,
            Description = request.Description ?? existing.Value.Description,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            Tags = request.Tags
        };

        return await transactions.SaveAsync(request.User, [modified], null, cancellationToken);
    }
}
