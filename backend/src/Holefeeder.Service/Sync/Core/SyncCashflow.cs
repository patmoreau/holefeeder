using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal sealed record SyncCashflow(
    Id Id,
    DateOnly EffectiveDate,
    IntervalType IntervalType,
    int Frequency,
    int Recurrence,
    Money Amount,
    string Description,
    Id AccountId,
    Id CategoryId,
    bool Inactive,
    TagList Tags) : ISyncRecord
{
    public SyncTable Table => SyncTable.Cashflows;

    public Result<ISyncRecord> Validated() =>
        SyncRecordErrors.Validated(this,
            (EffectiveDate != default, SyncRecordErrors.CashflowEffectiveDateRequired),
            (Frequency > 0, SyncRecordErrors.CashflowFrequencyInvalid),
            (Recurrence >= 0, SyncRecordErrors.CashflowRecurrenceInvalid),
            (AccountId != Id.Empty, SyncRecordErrors.CashflowAccountRequired),
            (CategoryId != Id.Empty, SyncRecordErrors.CashflowCategoryRequired));
}

internal sealed record SyncCashflowPatch(
    DateOnly? EffectiveDate = null,
    IntervalType? IntervalType = null,
    int? Frequency = null,
    int? Recurrence = null,
    Money? Amount = null,
    string? Description = null,
    Id? AccountId = null,
    Id? CategoryId = null,
    bool? Inactive = null,
    TagList? Tags = null) : ISyncPatch
{
    public SyncTable Table => SyncTable.Cashflows;

    public Result<ISyncRecord> Create(Id id) =>
        new SyncCashflow(
            id,
            EffectiveDate ?? default,
            IntervalType ?? IntervalType.Monthly,
            Frequency ?? 1,
            Recurrence ?? 0,
            Amount ?? Money.Zero,
            Description ?? string.Empty,
            AccountId ?? Id.Empty,
            CategoryId ?? Id.Empty,
            Inactive ?? false,
            Tags ?? TagList.Create([])).Validated();

    public Result<ISyncRecord> ApplyTo(ISyncRecord existing)
    {
        var cashflow = (SyncCashflow)existing;
        return (cashflow with
        {
            EffectiveDate = EffectiveDate ?? cashflow.EffectiveDate,
            IntervalType = IntervalType ?? cashflow.IntervalType,
            Frequency = Frequency ?? cashflow.Frequency,
            Recurrence = Recurrence ?? cashflow.Recurrence,
            Amount = Amount ?? cashflow.Amount,
            Description = Description ?? cashflow.Description,
            AccountId = AccountId ?? cashflow.AccountId,
            CategoryId = CategoryId ?? cashflow.CategoryId,
            Inactive = Inactive ?? cashflow.Inactive,
            Tags = Tags ?? cashflow.Tags
        }).Validated();
    }

    public ISyncRecord? KeptOnDelete(ISyncRecord existing) => null;
}
