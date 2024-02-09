using System.Text.Json.Serialization;

using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Holefeeder.Domain.Enumerations;

[JsonConverter(typeof(SmartEnumNameConverter<DateIntervalType, int>))]
public abstract class DateIntervalType : SmartEnum<DateIntervalType>
{
    public static readonly DateIntervalType Daily = new DailyDateIntervalType(nameof(Daily), 0);
    public static readonly DateIntervalType Weekly = new WeeklyDateIntervalType(nameof(Weekly), 1);
    public static readonly DateIntervalType Monthly = new MonthlyDateIntervalType(nameof(Monthly), 2);
    public static readonly DateIntervalType Yearly = new YearlyDateIntervalType(nameof(Yearly), 3);
    public static readonly DateIntervalType OneTime = new OneTimeDateIntervalType(nameof(OneTime), 4);

    public static (DateIntervalType IntervalType, int Frequency) GetIntervalTypeFromRange(DateOnly from, DateOnly to)
    {
        if (YearlyDateIntervalType.IsYearlyRange(from, to, out var frequency))
        {
            return (Yearly, frequency);
        }

        if (MonthlyDateIntervalType.IsMonthlyRange(from, to, out frequency))
        {
            return (Monthly, frequency);
        }

        return WeeklyDateIntervalType.IsWeeklyRange(from, to, out frequency)
            ? (Weekly, frequency)
            : (Daily, (to.ToDateTime(TimeOnly.MinValue) - from.ToDateTime(TimeOnly.MinValue)).Days + 1);
    }

    private DateIntervalType(string name, int id) : base(name, id)
    {
    }

    public abstract DateOnly AddIteration(DateOnly effectiveDate, int iteration);

    public virtual (DateOnly from, DateOnly to) Interval(DateOnly effectiveDate, DateOnly lookupDate, int frequency)
    {
        var next = lookupDate > effectiveDate;
        var start = effectiveDate;
        var end = AddIteration(start, frequency).AddDays(-1);
        var iteration = 1;
        while (start > lookupDate || end < lookupDate)
        {
            start = AddIteration(effectiveDate, (next ? frequency : -frequency) * iteration);
            end = AddIteration(start, frequency).AddDays(-1);
            iteration++;
        }

        return (start, end);
    }

    public virtual DateOnly NextDate(DateOnly effectiveDate, DateOnly fromDate, int frequency)
    {
        var start = effectiveDate;

        var count = 0;
        while (start < fromDate)
        {
            start = AddIteration(effectiveDate, frequency * count);
            count++;
        }

        return start;
    }

    public virtual DateOnly PreviousDate(DateOnly effectiveDate, DateOnly fromDate, int frequency)
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
            start = AddIteration(effectiveDate, frequency * count);
        }

        return AddIteration(effectiveDate, frequency * (count - 1));
    }

    public virtual IEnumerable<DateOnly> DatesInRange(DateOnly effectiveDate, DateOnly fromDate, DateOnly toDate,
        int frequency)
    {
        List<DateOnly> dates = [];
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

    private sealed class DailyDateIntervalType(string name, int id) : DateIntervalType(name, id)
    {
        public override DateOnly AddIteration(DateOnly effectiveDate, int iteration) =>
            effectiveDate.AddDays(iteration);
    }

    private sealed class WeeklyDateIntervalType(string name, int id) : DateIntervalType(name, id)
    {
        public override DateOnly AddIteration(DateOnly effectiveDate, int iteration) =>
            AddWeeks(effectiveDate, iteration);

        private static DateOnly AddWeeks(DateOnly effectiveDate, int weeks) => effectiveDate.AddDays(7 * weeks);

        internal static bool IsWeeklyRange(DateOnly fromDate, DateOnly toDate, out int frequency)
        {
            var weekly = false;
            frequency = 0;

            var start = fromDate;
            while (start < toDate)
            {
                frequency++;
                start = AddWeeks(start, 1);
                weekly = start.AddDays(-1) == toDate;
            }

            return weekly;
        }
    }

    private sealed class MonthlyDateIntervalType(string name, int id) : DateIntervalType(name, id)
    {
        public override DateOnly AddIteration(DateOnly effectiveDate, int iteration) =>
            effectiveDate.AddMonths(iteration);

        internal static bool IsMonthlyRange(DateOnly fromDate, DateOnly toDate, out int frequency)
        {
            var monthly = false;
            frequency = 0;

            var start = fromDate;
            while (start < toDate)
            {
                frequency++;
                start = start.AddMonths(1);
                monthly = start.AddDays(-1) == toDate;
            }

            return monthly;
        }
    }

    private sealed class YearlyDateIntervalType(string name, int id) : DateIntervalType(name, id)
    {
        public override DateOnly AddIteration(DateOnly effectiveDate, int iteration) =>
            effectiveDate.AddYears(iteration);

        internal static bool IsYearlyRange(DateOnly fromDate, DateOnly toDate, out int frequency)
        {
            var yearly = false;
            frequency = 0;

            var start = fromDate;
            while (start < toDate)
            {
                frequency++;
                start = start.AddYears(1);
                yearly = start.AddDays(-1) == toDate;
            }

            return yearly;
        }
    }

    private sealed class OneTimeDateIntervalType(string name, int id) : DateIntervalType(name, id)
    {
        public override DateOnly AddIteration(DateOnly effectiveDate, int iteration) => effectiveDate;

        public override DateOnly NextDate(DateOnly effectiveDate, DateOnly fromDate, int frequency) => effectiveDate;

        public override DateOnly PreviousDate(DateOnly effectiveDate, DateOnly fromDate, int frequency) =>
            effectiveDate;

        public override IEnumerable<DateOnly> DatesInRange(DateOnly effectiveDate, DateOnly fromDate, DateOnly toDate,
            int frequency) =>
            effectiveDate >= fromDate && effectiveDate <= toDate
                ? [effectiveDate]
                : Array.Empty<DateOnly>();

        public override (DateOnly from, DateOnly to) Interval(DateOnly effectiveDate, DateOnly lookupDate,
            int frequency) =>
            (effectiveDate, effectiveDate);
    }
}
