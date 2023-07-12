using System.Globalization;

namespace Holefeeder.Domain.Extensions;

public static class DateExtensions
{
    private const string DATE_FORMAT = "yyyy-MM-dd";

    public static string ToPersistent(this DateOnly date) => date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

    public static DateOnly ParsePersistent(this string persistentDate) =>
        DateOnly.ParseExact(persistentDate, DATE_FORMAT, null);
}
