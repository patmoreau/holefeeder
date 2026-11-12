using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Persistence;

internal sealed record MyDataAccountRow(
    Guid Id, string Type, string Name, decimal OpenBalance, DateOnly OpenDate, string? Description, bool Favorite, bool Inactive)
{
    public MyDataAccount ToRecord() =>
        new(Shared.Core.Id.Valid(Id), AccountType.Valid(Type), Name, Money.Valid(OpenBalance), OpenDate, Description ?? string.Empty,
            Favorite, Inactive);
}

internal sealed record MyDataCategoryRow(
    Guid Id, string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive)
{
    public MyDataCategory ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Enum.Parse<CategoryType>(Type, true), Name, Color, Money.Valid(BudgetAmount), Favorite, System,
            Inactive);
}

internal sealed record MyDataCashflowRow(
    Guid Id,
    DateOnly EffectiveDate,
    decimal Amount,
    string IntervalType,
    int Frequency,
    int Recurrence,
    string? Description,
    string? Tags,
    Guid CategoryId,
    Guid AccountId,
    bool Inactive)
{
    public MyDataCashflow ToRecord() =>
        new(Shared.Core.Id.Valid(Id), EffectiveDate, Money.Valid(Amount), Shared.Core.IntervalType.Valid(IntervalType), Frequency,
            Recurrence, Description ?? string.Empty, TagList.Create(Tags?.Split(',')), Shared.Core.Id.Valid(CategoryId),
            Shared.Core.Id.Valid(AccountId), Inactive);
}

internal sealed record MyDataTransactionRow(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Description,
    string? Tags,
    Guid CategoryId,
    Guid AccountId,
    Guid? CashflowId,
    DateOnly? CashflowDate)
{
    public MyDataTransaction ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Date, Money.Valid(Amount), Description ?? string.Empty, TagList.Create(Tags?.Split(',')),
            Shared.Core.Id.Valid(CategoryId), Shared.Core.Id.Valid(AccountId),
            CashflowId is { } cashflowId ? Shared.Core.Id.Valid(cashflowId) : null, CashflowDate);
}
