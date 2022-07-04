using System.Text.Json.Serialization;

using Holefeeder.Domain.SeedWork;
using Holefeeder.Domain.SeedWork.Converters;

namespace Holefeeder.Domain.Enumerations;

[JsonConverter(typeof(EnumerationJsonConverter<DateIntervalType>))]
public abstract class DateIntervalType : Enumeration
{
    public static readonly DateIntervalType Weekly = new WeeklyDateIntervalType(1, nameof(Weekly));
    public static readonly DateIntervalType Monthly = new MonthlyDateIntervalType(2, nameof(Monthly));
    public static readonly DateIntervalType Yearly = new YearlyDateIntervalType(3, nameof(Yearly));
    public static readonly DateIntervalType OneTime = new OneTimeDateIntervalType(4, nameof(OneTime));

    private DateIntervalType(int id, string name) : base(id, name) { }

    protected abstract DateTime AddIteration(DateTime date, int iteration);

    public abstract (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate, int frequency);

    public virtual DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency)
    {
        var start = effectiveDate;
        var next = fromDate;

        var count = 0;
        while (start < next)
        {
            start = AddIteration(effectiveDate, frequency * count);
            count++;
        }

        return start;
    }

    public virtual DateTime PreviousDate(DateTime effectiveDate, DateTime fromDate, int frequency)
    {
        if (effectiveDate > fromDate)
        {
            return effectiveDate;
        }

        var start = effectiveDate;
        var next = fromDate;

        var count = 0;
        while (start < next)
        {
            count++;
            start = AddIteration(effectiveDate, frequency * count);
        }

        return AddIteration(effectiveDate, frequency * (count - 1));
    }

    public virtual IEnumerable<DateTime> DatesInRange(DateTime effective, DateTime from, DateTime to, int frequency)
    {
        var dates = new List<DateTime>();
        var start = effective;

        var iteration = 1;
        while (start < from)
        {
            start = AddIteration(effective, frequency * iteration);
            iteration++;
        }

        while (start <= to)
        {
            dates.Add(start);
            start = AddIteration(effective, frequency * iteration);
            iteration++;
        }

        return dates;
    }

    private sealed class WeeklyDateIntervalType : DateIntervalType
    {
        public WeeklyDateIntervalType(int id, string name) : base(id, name) { }

        protected override DateTime AddIteration(DateTime date, int iteration)
        {
            return AddWeeks(date, iteration);
        }

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            var start = effectiveDate;
            var end = NextDate(effectiveDate, fromDate, frequency);

            if (end == start)
            {
                end = AddWeeks(end, frequency);
            }
            else
            {
                start = AddWeeks(end, -frequency);
            }

            return (start, end.AddDays(-1));
        }

        private static DateTime AddWeeks(DateTime date, int weeks)
        {
            return date.AddDays(7 * weeks);
        }
    }

    private sealed class MonthlyDateIntervalType : DateIntervalType
    {
        public MonthlyDateIntervalType(int id, string name) : base(id, name) { }

        protected override DateTime AddIteration(DateTime date, int iteration)
        {
            return date.AddMonths(iteration);
        }

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            var start = effectiveDate;
            var end = NextDate(effectiveDate, fromDate, frequency);

            if (end == start)
            {
                end = end.AddMonths(frequency);
            }
            else
            {
                start = end.AddMonths(-frequency);
            }

            return (start, end.AddDays(-1));
        }
    }

    private sealed class YearlyDateIntervalType : DateIntervalType
    {
        public YearlyDateIntervalType(int id, string name) : base(id, name) { }

        protected override DateTime AddIteration(DateTime date, int iteration)
        {
            return date.AddYears(iteration);
        }

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            var start = effectiveDate;
            var end = NextDate(effectiveDate, fromDate, frequency);

            if (end == start)
            {
                end = end.AddYears(frequency);
            }
            else
            {
                start = end.AddYears(-frequency);
            }

            return (start, end.AddDays(-1));
        }
    }

    private sealed class OneTimeDateIntervalType : DateIntervalType
    {
        public OneTimeDateIntervalType(int id, string name) : base(id, name) { }

        protected override DateTime AddIteration(DateTime date, int iteration)
        {
            return date;
        }

        public override DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency)
        {
            return effectiveDate;
        }

        public override DateTime PreviousDate(DateTime effectiveDate, DateTime fromDate, int frequency)
        {
            return effectiveDate;
        }

        public override IEnumerable<DateTime> DatesInRange(DateTime effective, DateTime from, DateTime to,
            int frequency)
        {
            return effective >= from && effective <= to ? new[] {effective} : Array.Empty<DateTime>();
        }

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            return (effectiveDate, effectiveDate);
        }
    }
}
