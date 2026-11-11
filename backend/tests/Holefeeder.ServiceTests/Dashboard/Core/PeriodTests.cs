using System.Globalization;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Dashboard.Core;

public class PeriodTests
{
    [Theory]
    [InlineData("2025-01-15", "Monthly", 1, "2026-11-10", "2026-10-15", "2026-11-14")]
    [InlineData("2025-01-15", "Monthly", 1, "2026-11-15", "2026-11-15", "2026-12-14")]
    [InlineData("2026-01-01", "Weekly", 2, "2026-01-20", "2026-01-15", "2026-01-28")]
    [InlineData("2020-04-01", "Yearly", 1, "2026-03-31", "2025-04-01", "2026-03-31")]
    [InlineData("2026-01-01", "Daily", 3, "2026-01-08", "2026-01-07", "2026-01-09")]
    [InlineData("2026-06-01", "Monthly", 1, "2026-04-20", "2026-04-01", "2026-04-30")]
    [InlineData("2026-06-01", "OneTime", 1, "2026-11-10", "2026-06-01", "2026-06-01")]
    public void Containing_ReturnsThePeriodThatHoldsTheDate(
        string effectiveDate, string intervalType, int frequency, string date, string expectedStart, string expectedEnd)
    {
        var settings = PeriodSettings.Valid(Day(effectiveDate), IntervalType.Create(intervalType).Value, frequency);

        var period = Period.Containing(Day(date), settings);

        period.Should().Be(new Period(Day(expectedStart), Day(expectedEnd)));
    }

    private static DateOnly Day(string value) => DateOnly.Parse(value, CultureInfo.InvariantCulture);
}
