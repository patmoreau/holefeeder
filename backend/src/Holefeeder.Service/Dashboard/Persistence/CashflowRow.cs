using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record CashflowRow(
    Guid Id,
    Guid AccountId,
    DateOnly EffectiveDate,
    string IntervalType,
    int Frequency,
    decimal Amount,
    string CategoryType,
    DateOnly? LastPaidDate,
    DateOnly? LastCashflowDate,
    string? Description,
    string CategoryName,
    string CategoryColor,
    string? Tags)
{
    public Cashflow ToCashflow() =>
        new(
            Shared.Core.Id.Valid(AccountId),
            EffectiveDate,
            Shared.Core.IntervalType.Valid(IntervalType),
            Frequency,
            Amount,
            Enum.Parse<CategoryType>(CategoryType, ignoreCase: true),
            LastPaidDate,
            LastCashflowDate);

    public UpcomingCashflow ToUpcomingCashflow() =>
        new(
            Shared.Core.Id.Valid(Id),
            ToCashflow(),
            Description ?? string.Empty,
            CategoryName,
            CategoryColor,
            (Tags ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
