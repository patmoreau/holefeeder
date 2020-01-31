using System;
using System.Globalization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Common.Extensions
{
	public static class DateHelper
	{
		public static string ToPersistent (this DateTime date) => date.ToString ("yyyy-MM-dd");

		public static DateTime ParsePersistent(this string persistentDate)
		=> DateTime.ParseExact(persistentDate, "yyyy-MM-dd", null, DateTimeStyles.None);

		public static DateTime AddWeeks (this DateTime date, int weeks) => date.AddDays (7 * weeks);

		public static Func<DateTime, int, DateTime> NextIterationMethod (DateIntervalType intervalType)
		{
			if (intervalType == DateIntervalType.Weekly) {
				return (date, intervalCount) => date.AddWeeks (intervalCount);
			}
			if (intervalType == DateIntervalType.Monthly) {
				return (date, intervalCount) => date.AddMonths (intervalCount);
			} 
			return (date, intervalCount) => date.AddYears (intervalCount);
		}

		public static DateTime NextDate (this DateTime self, DateTime effectiveDate, DateIntervalType intervalType, int frequency)
		{
			if (intervalType == DateIntervalType.OneTime) {
				return self;
			}

			var nextIteration = NextIterationMethod(intervalType);

			var start = self;
			var next = effectiveDate;

			int count = 0;
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

			int count = 0;
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
			}

			return (start, end.AddDays (-1));
		}
	}
}

