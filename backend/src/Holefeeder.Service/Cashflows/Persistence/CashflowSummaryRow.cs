using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Persistence;

internal sealed record CashflowSummaryRow(
    Guid Id,
    string? Description,
    Guid CategoryId,
    Guid AccountId,
    DateOnly EffectiveDate,
    string IntervalType,
    int Frequency,
    decimal Amount,
    string CategoryType,
    DateOnly? LastPaidDate,
    DateOnly? LastCashflowDate,
    string CategoryName,
    string CategoryColor,
    string AccountName,
    string? Tags)
{
    public CashflowSummary ToCashflowSummary() =>
        new(
            Shared.Core.Id.Valid(Id),
            Description ?? string.Empty,
            Shared.Core.Id.Valid(CategoryId),
            new CashflowSchedule(
                Shared.Core.Id.Valid(AccountId),
                EffectiveDate,
                Shared.Core.IntervalType.Valid(IntervalType),
                Frequency,
                Amount,
                Enum.Parse<CategoryType>(CategoryType, ignoreCase: true),
                LastPaidDate,
                LastCashflowDate),
            CategoryName,
            CategoryColor,
            AccountName,
            (Tags ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
