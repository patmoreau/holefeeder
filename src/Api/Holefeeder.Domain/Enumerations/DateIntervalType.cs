using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Holefeeder.Domain.Enumerations;

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
        DateTime start = effectiveDate;
        DateTime next = fromDate;

        int count = 0;
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

        DateTime start = effectiveDate;

        int count = 0;
        while (start < fromDate)
        {
            count++;
            start = AddIteration(effectiveDate, frequency * count);
        }

        return AddIteration(effectiveDate, frequency * (count - 1));
    }

    public virtual IEnumerable<DateTime> DatesInRange(DateTime effectiveDate, DateTime fromDate, DateTime toDate,
        int frequency)
    {
        List<DateTime> dates = new List<DateTime>();
        DateTime start = effectiveDate;

        int iteration = 1;
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

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration) =>
            AddWeeks(effectiveDate, iteration);

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            DateTime start = effectiveDate;
            DateTime end = NextDate(effectiveDate, fromDate, frequency);

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

        private static DateTime AddWeeks(DateTime effectiveDate, int weeks) => effectiveDate.AddDays(7 * weeks);
    }

    private sealed class MonthlyDateIntervalType : DateIntervalType
    {
        public MonthlyDateIntervalType(string name, int id)
            : base(name, id)
        {
        }

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration) =>
            effectiveDate.AddMonths(iteration);

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            DateTime start = effectiveDate;
            DateTime end = NextDate(effectiveDate, fromDate, frequency);

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

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration) =>
            effectiveDate.AddYears(iteration);

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency)
        {
            DateTime start = effectiveDate;
            DateTime end = NextDate(effectiveDate, fromDate, frequency);

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

        protected override DateTime AddIteration(DateTime effectiveDate, int iteration) => effectiveDate;

        public override DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency) => effectiveDate;

        public override DateTime PreviousDate(DateTime effectiveDate, DateTime fromDate, int frequency) =>
            effectiveDate;

        public override IEnumerable<DateTime> DatesInRange(DateTime effectiveDate, DateTime fromDate, DateTime toDate,
            int frequency) =>
            effectiveDate >= fromDate && effectiveDate <= toDate
                ? new[] { effectiveDate }
                : Array.Empty<DateTime>();

        public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
            int frequency) =>
            (effectiveDate, effectiveDate);
    }
}
