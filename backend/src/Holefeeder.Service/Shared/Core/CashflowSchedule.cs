
namespace Holefeeder.Service.Shared.Core;

internal sealed record CashflowSchedule(
    Id AccountId,
    DateOnly EffectiveDate,
    IntervalType IntervalType,
    int Frequency,
    decimal Amount,
    CategoryType CategoryType,
    DateOnly? LastPaidDate,
    DateOnly? LastCashflowDate)
{
    public IReadOnlyList<DateOnly> UpcomingDates(DateOnly to) =>
        [.. Occurrences(to).Where(IsUnpaid)];

    public decimal UpcomingVariation(DateOnly to) =>
        UpcomingDates(to).Count * Amount * (CategoryType == CategoryType.Gain ? 1 : -1);

    private IEnumerable<DateOnly> Occurrences(DateOnly to)
    {
        if (IntervalType == IntervalType.OneTime)
        {
            if (EffectiveDate <= to)
            {
                yield return EffectiveDate;
            }

            yield break;
        }

        for (var iteration = 0; IntervalType.AddIteration(EffectiveDate, Frequency * iteration) <= to; iteration++)
        {
            yield return IntervalType.AddIteration(EffectiveDate, Frequency * iteration);
        }
    }

    private bool IsUnpaid(DateOnly date) =>
        LastPaidDate is { } lastPaidDate && LastCashflowDate is { } lastCashflowDate
            ? date > lastPaidDate && date > lastCashflowDate
            : date >= EffectiveDate;
}
