namespace Holefeeder.Domain.Enumerations;

using System.Text.Json.Serialization;

using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

[JsonConverter(typeof(SmartEnumNameConverter<DateIntervalType, int>))]
public abstract class DateIntervalType : SmartEnum<DateIntervalType>
{
    public static readonly DateIntervalType Weekly = new WeeklyDateIntervalType(nameof(Weekly), 1);
    public static readonly DateIntervalType Monthly = new MonthlyDateIntervalType(nameof(Monthly), 2);
    public static readonly DateIntervalType Yearly = new YearlyDateIntervalType(nameof(Yearly), 3);
    public static readonly DateIntervalType OneTime = new OneTimeDateIntervalType(nameof(OneTime), 4);

    private DateIntervalType(string name, int id)
        : base(name, id)
    {
    }

    protected abstract DateTime AddIteration(DateTime effectiveDate, int iteration);

    public abstract (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate, int frequency);

    public virtual DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency)
    {
        var start = effectiveDate;
        var next = fromDate;

        var count = 0;
        while (start < next)
        {
            start = this.AddIteration(effectiveDate, frequency * count);
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

        var count = 0;
        while (start < fromDate)
        {
            count++;
            start = this.AddIteration(effectiveDate, frequency * count);
        }

        return this.AddIteration(effectiveDate, frequency * (count - 1));
    }

    public virtual IEnumerable<DateTime> DatesInRange(DateTime effectiveDate, DateTime fromDate, DateTime toDate,
        int frequency)
    {
        var dates = new List<DateTime>();
        var start = effectiveDate;

        var iteration = 1;
        while (start < fromDate)
        {
            start = AddIteration(effectiveDate, frequency * iteration);
            iteration++;
        }

        while (start <= toDate)
        {
            dates.Add(start);
            start = AddIteration(effectiveDate, frequency * iteration);
            iteration++;
        }

        return dates;
    }

    private sealed class WeeklyDateIntervalType : DateIntervalType
    {
        public WeeklyDateIntervalType(string name, int id)
            : base(name, id)
        {
        }

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration)
        {
            return AddWeeks(effectiveDate, iteration);
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

        private static DateTime AddWeeks(DateTime effectiveDate, int weeks)
        {
            return effectiveDate.AddDays(7 * weeks);
        }
    }

    private sealed class MonthlyDateIntervalType : DateIntervalType
    {
        public MonthlyDateIntervalType(string name, int id)
            : base(name, id)
        {
        }

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration)
        {
            return effectiveDate.AddMonths(iteration);
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
        public YearlyDateIntervalType(string name, int id)
            : base(name, id)
        {
        }

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration)
        {
            return effectiveDate.AddYears(iteration);
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
        public OneTimeDateIntervalType(string name, int id)
            : base(name, id)
        {
        }

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration)
        {
            return effectiveDate;
        }

        public override DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency)
        {
            return effectiveDate;
        }

        public override DateTime PreviousDate(DateTime effectiveDate, DateTime fromDate, int frequency)
        {
            return effectiveDate;
        }

        public override IEnumerable<DateTime> DatesInRange(DateTime effectiveDate, DateTime fromDate, DateTime toDate,
            int frequency)
        {
            return effectiveDate >= fromDate && effectiveDate <= toDate
                ? new[] {effectiveDate}
                : Array.Empty<DateTime>();
        }

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            return (effectiveDate, effectiveDate);
        }
    }
}
