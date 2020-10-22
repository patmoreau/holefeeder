using System;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.Enumerations
{
    public abstract class DateIntervalType : Enumeration
    {
        public static readonly DateIntervalType Weekly = new WeeklyDateIntervalType(1, nameof(Weekly));
        public static readonly DateIntervalType Monthly = new MonthlyDateIntervalType(2, nameof(Monthly));
        public static readonly DateIntervalType Yearly = new YearlyDateIntervalType(3, nameof(Yearly));
        public static readonly DateIntervalType OneTime = new OneTimeDateIntervalType(4, nameof(OneTime));

        private DateIntervalType(int id, string name) : base(id, name) { }

        protected abstract DateTime AddIteration(DateTime startDate, int iteration);

        public abstract (DateTime from, DateTime to) Interval(DateTime startDate, DateTime effectiveDate, int frequency);

        private class WeeklyDateIntervalType : DateIntervalType
        {
            public WeeklyDateIntervalType(int id, string name) : base(id, name) { }
            protected override DateTime AddIteration(DateTime startDate, int iteration) => AddWeeks(startDate, iteration);

            public override (DateTime from, DateTime to) Interval(DateTime startDate, DateTime effectiveDate, int frequency)
            {
                var start = startDate;
                var end = NextDate(startDate, effectiveDate, frequency);

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

            protected override DateTime AddIteration(DateTime startDate, int iteration) => startDate.AddMonths(iteration);

            public override (DateTime from, DateTime to) Interval(DateTime startDate, DateTime effectiveDate, int frequency)
            {
                var start = startDate;
                var end = NextDate(startDate, effectiveDate, frequency);

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

            protected override DateTime AddIteration(DateTime startDate, int iteration) => startDate.AddYears(iteration);

            public override (DateTime from, DateTime to) Interval(DateTime startDate, DateTime effectiveDate, int frequency)
            {
                var start = startDate;
                var end = NextDate(startDate, effectiveDate, frequency);

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

            protected override DateTime AddIteration(DateTime startDate, int iteration) => startDate;
            public override DateTime NextDate(DateTime startDate, DateTime effectiveDate, int frequency) => startDate;
            public override DateTime PreviousDate(DateTime startDate, DateTime effectiveDate, int frequency) => startDate;

            public override (DateTime from, DateTime to) Interval(DateTime startDate, DateTime effectiveDate, int frequency)
            {
                return (startDate, startDate);
            }
        }

        public virtual DateTime NextDate(DateTime startDate, DateTime effectiveDate, int frequency)
        {
            var start = startDate;
            var next = effectiveDate;

            var count = 0;
            while (start < next)
            {
                start = AddIteration(startDate, frequency * count);
                count++;
            }

            return start;
        }

        public virtual DateTime PreviousDate(DateTime startDate, DateTime effectiveDate, int frequency)
        {
            if (startDate > effectiveDate)
            {
                return startDate;
            }

            var start = startDate;
            var next = effectiveDate;

            var count = 0;
            while (start < next)
            {
                count++;
                start = AddIteration(startDate, frequency * count);
            }

            return AddIteration(startDate, frequency * (count - 1));
        }
    }
}
