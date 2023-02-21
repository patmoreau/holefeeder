using System.Globalization;

namespace Holefeeder.Domain.Extensions;

public static class DateExtensions
{
    private const string DATE_FORMAT = "yyyy-MM-dd";

    public static string ToPersistent(this DateTime date)
    {
        return date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
    }

    public static DateTime ParsePersistent(this string persistentDate)
    {
        return DateTime.ParseExact(persistentDate, DATE_FORMAT, null, DateTimeStyles.None);
    }
}
