using System;
using System.Globalization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Common.Extensions
{
	public static class DateHelper
	{
		private const string DATE_FORMAT = "yyyy-MM-dd";
		public static string ToPersistent (this DateTime date) => date.ToString (DATE_FORMAT, CultureInfo.InvariantCulture);

		public static DateTime ParsePersistent(this string persistentDate)
		=> DateTime.ParseExact(persistentDate, DATE_FORMAT, null, DateTimeStyles.None);

		private static DateTime AddWeeks (this DateTime date, int weeks) => date.AddDays (7 * weeks);

		private static Func<DateTime, int, DateTime> NextIterationMethod (DateIntervalType intervalType)
		{
			return intervalType switch
			{
				DateIntervalType.Weekly => ((date, intervalCount) =>
                    date.AddWeeks(intervalCount)),
				DateIntervalType.Monthly => ((date, intervalCount) => date.AddMonths(intervalCount)),
				_ => ((date, intervalCount) => date.AddYears(intervalCount))
			};
		}

		public static DateTime NextDate (this DateTime self, DateTime effectiveDate, DateIntervalType intervalType, int frequency)
		{
			if (intervalType == DateIntervalType.OneTime) {
				return self;
			}

			var nextIteration = NextIterationMethod(intervalType);

			var start = self;
			var next = effectiveDate;

			var count = 0;
			while (start < next) {
				start = nextIteration (self, frequency * count);
				count++;
			}
			return start;
		}

		public static DateTime PreviousDate (this DateTime self, DateTime effectiveDate, DateIntervalType intervalType, int frequency)
		{
			if (intervalType == DateIntervalType.OneTime)
			{
				return self;
			}
			if (self > effectiveDate)
			{
				return self;
			}

			var nextIteration = NextIterationMethod (intervalType);

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

		public static (DateTime from, DateTime to) Interval (this DateTime date, DateTime effectiveDate, DateIntervalType intervalType, int frequency)
		{
			var start = date;
			var end = date.NextDate(effectiveDate, intervalType, frequency);
			switch (intervalType)
			{
				case DateIntervalType.Weekly:
					if (end == start)
					{
						end = end.AddWeeks(frequency);
					}
					else
					{
						start = end.AddWeeks(-frequency);
					}
					break;
				case DateIntervalType.Monthly:
					if (end == start)
					{
						end = end.AddMonths(frequency);
					}
					else
					{
						start = end.AddMonths(-frequency);
					}
					break;
				case DateIntervalType.Yearly:
					if (end == start)
					{
						end = end.AddYears(frequency);
					}
					else
					{
						start = end.AddYears(-frequency);
					}
					break;
				case DateIntervalType.OneTime:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(intervalType), intervalType, null);
			}

			return (start, end.AddDays (-1));
		}
	}
}

