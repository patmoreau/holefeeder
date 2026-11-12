
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

    public DateOnly? NextDate() =>
        Occurrences().Where(IsUnpaid).Select(date => (DateOnly?)date).FirstOrDefault();

    private IEnumerable<DateOnly> Occurrences(DateOnly to) =>
        Occurrences().TakeWhile(date => date <= to);

    private IEnumerable<DateOnly> Occurrences()
    {
        if (IntervalType == IntervalType.OneTime)
        {
            yield return EffectiveDate;
            yield break;
        }

        for (var iteration = 0; ; iteration++)
        {
            yield return IntervalType.AddIteration(EffectiveDate, Frequency * iteration);
        }
    }

    private bool IsUnpaid(DateOnly date) =>
        LastPaidDate is { } lastPaidDate && LastCashflowDate is { } lastCashflowDate
            ? date > lastPaidDate && date > lastCashflowDate
            : date >= EffectiveDate;
}
