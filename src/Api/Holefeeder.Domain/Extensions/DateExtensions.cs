using System.Globalization;

namespace Holefeeder.Domain.Extensions;

public static class DateExtensions
{
    private const string DateFormat = "yyyy-MM-dd";

    public static string ToPersistent(this DateOnly date) => date.ToString(DateFormat, CultureInfo.InvariantCulture);

    public static DateOnly ParsePersistent(this string persistentDate) =>
        DateOnly.ParseExact(persistentDate, DateFormat, null);
}
