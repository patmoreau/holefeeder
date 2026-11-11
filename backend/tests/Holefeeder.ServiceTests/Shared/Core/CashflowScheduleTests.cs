using System.Globalization;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Shared.Core;

public class CashflowScheduleTests
{
    [Theory]
    [InlineData("2026-09-15", "Monthly", 1, null, null, "2026-11-30", "2026-09-15,2026-10-15,2026-11-15")]
    [InlineData("2026-09-15", "Monthly", 1, "2026-10-16", "2026-10-15", "2026-11-30", "2026-11-15")]
    [InlineData("2026-09-15", "Monthly", 1, "2026-11-14", "2026-11-15", "2026-11-30", "")]
    [InlineData("2026-11-01", "Weekly", 1, "2026-11-01", "2026-11-01", "2026-11-30", "2026-11-08,2026-11-15,2026-11-22,2026-11-29")]
    [InlineData("2026-11-02", "Weekly", 2, null, null, "2026-11-30", "2026-11-02,2026-11-16,2026-11-30")]
    [InlineData("2026-12-01", "Monthly", 1, null, null, "2026-11-30", "")]
    [InlineData("2026-11-20", "OneTime", 1, null, null, "2026-11-30", "2026-11-20")]
    [InlineData("2026-11-20", "OneTime", 1, "2026-11-20", "2026-11-20", "2026-11-30", "")]
    [InlineData("2026-12-20", "OneTime", 1, null, null, "2026-11-30", "")]
    public void UpcomingDates_ListsUnpaidOccurrencesUpToTheDate(
        string effectiveDate, string intervalType, int frequency, string? lastPaidDate, string? lastCashflowDate, string to, string expected)
    {
        var cashflow = new CashflowSchedule(
            AnId(),
            Day(effectiveDate),
            IntervalType.Create(intervalType).Value,
            frequency,
            100m,
            CategoryType.Expense,
            lastPaidDate is null ? null : Day(lastPaidDate),
            lastCashflowDate is null ? null : Day(lastCashflowDate));

        var dates = cashflow.UpcomingDates(Day(to));

        dates.Should().Equal(expected.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Day));
    }

    [Theory]
    [InlineData("Expense", -300)]
    [InlineData("Gain", 300)]
    public void UpcomingVariation_SignsTheAmountByCategoryType(string categoryType, decimal expected)
    {
        var cashflow = new CashflowSchedule(
            AnId(), new DateOnly(2026, 11, 1), IntervalType.Weekly, 1, 100m, Enum.Parse<CategoryType>(categoryType), null, null);

        cashflow.UpcomingVariation(new DateOnly(2026, 11, 20)).Should().Be(expected);
    }

    private static DateOnly Day(string value) => DateOnly.Parse(value, CultureInfo.InvariantCulture);
}
