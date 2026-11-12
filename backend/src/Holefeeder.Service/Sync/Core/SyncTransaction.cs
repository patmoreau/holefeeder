using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal sealed record SyncTransaction(
    Id Id,
    DateOnly Date,
    Money Amount,
    string Description,
    Id AccountId,
    Id CategoryId,
    Id? CashflowId,
    DateOnly? CashflowDate,
    TagList Tags) : ISyncRecord
{
    public SyncTable Table => SyncTable.Transactions;

    public Result<ISyncRecord> Validated() =>
        SyncRecordErrors.Validated(this,
            (Date != default, SyncRecordErrors.TransactionDateRequired),
            (AccountId != Id.Empty, SyncRecordErrors.TransactionAccountRequired),
            (CategoryId != Id.Empty, SyncRecordErrors.TransactionCategoryRequired));
}

internal sealed record SyncTransactionPatch(
    DateOnly? Date = null,
    Money? Amount = null,
    string? Description = null,
    Id? AccountId = null,
    Id? CategoryId = null,
    Id? CashflowId = null,
    DateOnly? CashflowDate = null,
    TagList? Tags = null) : ISyncPatch
{
    public SyncTable Table => SyncTable.Transactions;

    public Result<ISyncRecord> Create(Id id) =>
        new SyncTransaction(
            id,
            Date ?? default,
            Amount ?? Money.Zero,
            Description ?? string.Empty,
            AccountId ?? Id.Empty,
            CategoryId ?? Id.Empty,
            CashflowId,
            CashflowDate,
            Tags ?? TagList.Create([])).Validated();

    public Result<ISyncRecord> ApplyTo(ISyncRecord existing)
    {
        var transaction = (SyncTransaction)existing;
        return (transaction with
        {
            Date = Date ?? transaction.Date,
            Amount = Amount ?? transaction.Amount,
            Description = Description ?? transaction.Description,
            AccountId = AccountId ?? transaction.AccountId,
            CategoryId = CategoryId ?? transaction.CategoryId,
            CashflowId = CashflowId ?? transaction.CashflowId,
            CashflowDate = CashflowDate ?? transaction.CashflowDate,
            Tags = Tags ?? transaction.Tags
        }).Validated();
    }

    public ISyncRecord? KeptOnDelete(ISyncRecord existing) => null;
}
