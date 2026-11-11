using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Persistence;

internal sealed record CashflowRow(
    Guid Id,
    DateOnly EffectiveDate,
    decimal Amount,
    string IntervalType,
    int Frequency,
    int Recurrence,
    string? Description,
    Guid AccountId,
    Guid CategoryId,
    bool Inactive,
    string? Tags)
{
    public Cashflow ToCashflow() =>
        Cashflow.Valid(
            Shared.Core.Id.Valid(Id), EffectiveDate, Money.Valid(Amount), IntervalType, Frequency, Recurrence,
            Description ?? string.Empty, Shared.Core.Id.Valid(AccountId), Shared.Core.Id.Valid(CategoryId), Inactive, Tags ?? string.Empty);
}
