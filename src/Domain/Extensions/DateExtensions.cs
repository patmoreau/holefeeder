using System;
using System.Globalization;

namespace DrifterApps.Holefeeder.Domain.Extensions
{
    public static class DateExtensions
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";

        public static string ToPersistent(this DateTime date) =>
            date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

        public static DateTime ParsePersistent(this string persistentDate) =>
            DateTime.ParseExact(persistentDate, DATE_FORMAT, null, DateTimeStyles.None);
    }
}
