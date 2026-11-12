using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Sync.Core;

namespace Holefeeder.Service.Sync.Persistence;

internal sealed record SyncAccountRow(
    Guid Id, string Type, string Name, decimal OpenBalance, DateOnly OpenDate, string? Description, bool Favorite, bool Inactive)
{
    public SyncAccount ToRecord() =>
        new(Shared.Core.Id.Valid(Id), AccountType.Valid(Type), Name, Money.Valid(OpenBalance), OpenDate, Description ?? string.Empty,
            Favorite, Inactive);
}

internal sealed record SyncCategoryRow(
    Guid Id, string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive)
{
    public SyncCategory ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Enum.Parse<CategoryType>(Type, true), Name, Color, Money.Valid(BudgetAmount), Favorite, System,
            Inactive);
}

internal sealed record SyncStoreItemRow(Guid Id, string Code, string Data)
{
    public SyncStoreItem ToRecord() => new(Shared.Core.Id.Valid(Id), Code, Data);
}

internal sealed record SyncTransactionRow(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Description,
    Guid AccountId,
    Guid CategoryId,
    Guid? CashflowId,
    DateOnly? CashflowDate,
    string? Tags)
{
    public SyncTransaction ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Date, Money.Valid(Amount), Description ?? string.Empty, Shared.Core.Id.Valid(AccountId),
            Shared.Core.Id.Valid(CategoryId), CashflowId is { } cashflowId ? Shared.Core.Id.Valid(cashflowId) : null, CashflowDate,
            TagList.Create(Tags?.Split(',')));
}

internal sealed record SyncCashflowRow(
    Guid Id,
    DateOnly EffectiveDate,
    string IntervalType,
    int Frequency,
    int Recurrence,
    decimal Amount,
    string? Description,
    Guid AccountId,
    Guid CategoryId,
    bool Inactive,
    string? Tags)
{
    public SyncCashflow ToRecord() =>
        new(Shared.Core.Id.Valid(Id), EffectiveDate, Shared.Core.IntervalType.Valid(IntervalType), Frequency, Recurrence,
            Money.Valid(Amount), Description ?? string.Empty, Shared.Core.Id.Valid(AccountId), Shared.Core.Id.Valid(CategoryId), Inactive,
            TagList.Create(Tags?.Split(',')));
}
