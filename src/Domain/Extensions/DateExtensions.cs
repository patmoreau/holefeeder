using System;
using System.Globalization;
using DrifterApps.Holefeeder.Domain.AggregatesModel.CashflowAggregate;

namespace DrifterApps.Holefeeder.Domain.Extensions
{
    public static class DateExtensions
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";

        public static string ToPersistent(this DateTime date) =>
            date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

        public static DateTime ParsePersistent(this string persistentDate)
            => DateTime.ParseExact(persistentDate, DATE_FORMAT, null, DateTimeStyles.None);

        private static DateTime AddWeeks(this DateTime date, int weeks) => date.AddDays(7 * weeks);

        private static Func<DateTime, int, DateTime> NextIterationMethod(DateIntervalType intervalType)
        {
            if (Equals(intervalType, DateIntervalType.Weekly))
                return (date, intervalCount) => date.AddWeeks(intervalCount);
            if (Equals(intervalType, DateIntervalType.Monthly))
                return (date, intervalCount) => date.AddMonths(intervalCount);
            if (Equals(intervalType, DateIntervalType.Yearly))
                return (date, intervalCount) => date.AddYears(intervalCount);
            return (date, intervalCount) => date;
        }

        public static DateTime NextDate(this DateTime self, DateTime effectiveDate, DateIntervalType intervalType,
            int frequency)
        {
            if (Equals(intervalType, DateIntervalType.OneTime))
            {
                return self;
            }

            var nextIteration = NextIterationMethod(intervalType);

            var start = self;
            var next = effectiveDate;

            var count = 0;
            while (start < next)
            {
                start = nextIteration(self, frequency * count);
                count++;
            }

            return start;
        }

        public static DateTime PreviousDate(this DateTime self, DateTime effectiveDate, DateIntervalType intervalType,
            int frequency)
        {
            if (Equals(intervalType, DateIntervalType.OneTime))
            {
                return self;
            }

            if (self > effectiveDate)
            {
                return self;
            }

            var nextIteration = NextIterationMethod(intervalType);

            var start = self;
            var next = effectiveDate;

            var count = 0;
            while (start < next)
            {
                count++;
                start = nextIteration(self, frequency * count);
            }

            return nextIteration(self, frequency * (count - 1));
        }

        public static (DateTime from, DateTime to) Interval(this DateTime date, DateTime effectiveDate,
            DateIntervalType intervalType, int frequency)
        {
            var start = date;
            var end = date.NextDate(effectiveDate, intervalType, frequency);

            if (Equals(intervalType, DateIntervalType.Weekly))
            {
                if (end == start)
                {
                    end = end.AddWeeks(frequency);
                }
                else
                {
                    start = end.AddWeeks(-frequency);
                }
            }
            else if (Equals(intervalType, DateIntervalType.Monthly))
            {
                if (end == start)
                {
                    end = end.AddMonths(frequency);
                }
                else
                {
                    start = end.AddMonths(-frequency);
                }
            }
            else if (Equals(intervalType, DateIntervalType.Yearly))
            {
                if (end == start)
                {
                    end = end.AddYears(frequency);
                }
                else
                {
                    start = end.AddYears(-frequency);
                }
            }
            else if (Equals(intervalType, DateIntervalType.OneTime))
            {
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(intervalType), intervalType, null);
            }


            return (start, end.AddDays(-1));
        }
    }
}
