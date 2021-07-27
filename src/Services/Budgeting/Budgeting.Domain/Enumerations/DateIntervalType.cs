using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Converters;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Enumerations
{
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

        private class WeeklyDateIntervalType : DateIntervalType
        {
            public WeeklyDateIntervalType(int id, string name) : base(id, name) { }
            protected override DateTime AddIteration(DateTime date, int iteration) => AddWeeks(date, iteration);

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

            private static DateTime AddWeeks(DateTime date, int weeks) => date.AddDays(7 * weeks);
        }

        private class MonthlyDateIntervalType : DateIntervalType
        {
            public MonthlyDateIntervalType(int id, string name) : base(id, name) { }

            protected override DateTime AddIteration(DateTime date, int iteration) => date.AddMonths(iteration);

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

        private class YearlyDateIntervalType : DateIntervalType
        {
            public YearlyDateIntervalType(int id, string name) : base(id, name) { }

            protected override DateTime AddIteration(DateTime date, int iteration) => date.AddYears(iteration);

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

        private class OneTimeDateIntervalType : DateIntervalType
        {
            public OneTimeDateIntervalType(int id, string name) : base(id, name) { }

            protected override DateTime AddIteration(DateTime date, int iteration) => date;

            public override DateTime NextDate(DateTime effectiveDate, DateTime fromDate, int frequency) =>
                effectiveDate;

            public override DateTime PreviousDate(DateTime effectiveDate, DateTime fromDate, int frequency) =>
                effectiveDate;

            public override IEnumerable<DateTime> DatesInRange(DateTime effective, DateTime from, DateTime to,
                int frequency) => effective >= @from && effective <= to ? new[] {effective} : Array.Empty<DateTime>();

            public override (DateTime from, DateTime to) Interval(DateTime effectiveDate, DateTime fromDate,
                int frequency) => (effectiveDate, effectiveDate);
        }

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
    }
}
