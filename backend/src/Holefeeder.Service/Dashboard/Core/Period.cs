namespace Holefeeder.Service.Dashboard.Core;

internal sealed record Period(DateOnly Start, DateOnly End)
{
    public static Period Containing(DateOnly date, PeriodSettings settings)
    {
        var intervalType = settings.IntervalType;
        var effectiveDate = settings.EffectiveDate;
        if (intervalType == IntervalType.OneTime)
        {
            return new Period(effectiveDate, effectiveDate);
        }

        var step = date > effectiveDate ? settings.Frequency : -settings.Frequency;
        var start = effectiveDate;
        var end = intervalType.AddIteration(start, settings.Frequency).AddDays(-1);
        var iteration = 1;
        while (start > date || end < date)
        {
            start = intervalType.AddIteration(effectiveDate, step * iteration);
            end = intervalType.AddIteration(start, settings.Frequency).AddDays(-1);
            iteration++;
        }

        return new Period(start, end);
    }
}
