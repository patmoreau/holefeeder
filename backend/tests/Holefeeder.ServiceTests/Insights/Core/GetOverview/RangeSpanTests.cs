using Holefeeder.Service.Insights.Core.GetOverview;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Insights.Core.GetOverview;

public class RangeSpanTests
{
    [Theory]
    [InlineData("monthly", 1, 3, "Month", 3)]
    [InlineData("monthly", 1, 12, "Year", 1)]
    [InlineData("monthly", 2, 12, "Year", 2)]
    [InlineData("monthly", 3, 6, "Month", 18)]
    [InlineData("weekly", 2, 6, "Week", 12)]
    [InlineData("weekly", 1, 52, "Year", 1)]
    [InlineData("yearly", 1, 3, "Year", 3)]
    [InlineData("daily", 7, 6, "Day", 42)]
    [InlineData("oneTime", 1, 6, "Day", 6)]
    public void Of_DescribesTheRangeInTheBudgetsUnit_InYearsWhenWhole(
        string intervalType, int frequency, int count, string unit, int amount)
    {
        var settings = PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Valid(intervalType), frequency);

        RangeSpan.Of(count, settings).Should().Be(new RangeSpan(Enum.Parse<RangeUnit>(unit), amount));
    }
}
