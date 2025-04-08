using Holefeeder.Domain.Enumerations;

namespace Holefeeder.UnitTests.Domain.Enumerations;

[UnitTest, Category("Domain")]
public class DateIntervalTypeTests
{
    [Theory]
    [MemberData(nameof(FromValueTestCases))]
    public void GivenDateIntervalType_WhenFromValue_ThenReturnDateIntervalType(string name, DateIntervalType expected)
    {
        var result = DateIntervalType.FromValue(name);
        result.Should().Be(expected);
    }

    public static TheoryData<string, DateIntervalType> FromValueTestCases() =>
        new()
        {
            {nameof(DateIntervalType.Daily), DateIntervalType.Daily},
            {"daily", DateIntervalType.Daily},
            {nameof(DateIntervalType.Weekly), DateIntervalType.Weekly},
            {"weekly", DateIntervalType.Weekly},
            {nameof(DateIntervalType.Monthly), DateIntervalType.Monthly},
            {"monthly", DateIntervalType.Monthly},
            {nameof(DateIntervalType.Yearly), DateIntervalType.Yearly},
            {"yearly", DateIntervalType.Yearly},
            {nameof(DateIntervalType.OneTime), DateIntervalType.OneTime},
            {"onetime", DateIntervalType.OneTime}
        };

    public static TheoryData<DateOnly, DateOnly, DateIntervalType, int, DateOnly> NextDateTestCases()
    {
        var testCases = new TheoryData<DateOnly, DateOnly, DateIntervalType, int, DateOnly>
        {
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1, new DateOnly(2014, 1, 9) },
            { new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1, new DateOnly(2015, 9, 24) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1, new DateOnly(2015, 4, 9) },
            { new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1, new DateOnly(2015, 9, 24) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 2, new DateOnly(2015, 4, 16) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 5, new DateOnly(2015, 4, 9) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 1, new DateOnly(2015, 4, 9) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 1, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 1, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 2, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 2, 28) },
            { new DateOnly(2014, 1, 31), new DateOnly(2016, 2, 15), DateIntervalType.Monthly, 1, new DateOnly(2016, 2, 29) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 3, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 3, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 4, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 4, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 5, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 5, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 6, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 6, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 7, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 7, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 8, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 8, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 9, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 9, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 10, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 10, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 11, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 11, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 12, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 12, 31) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 2, new DateOnly(2015, 5, 9) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 1, new DateOnly(2016, 1, 9) },
            { new DateOnly(2014, 2, 28), new DateOnly(2016, 2, 1), DateIntervalType.Yearly, 1, new DateOnly(2016, 2, 28) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 7, new DateOnly(2021, 1, 9) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Daily, 3, new DateOnly(2015, 4, 7) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Daily, 300, new DateOnly(2015, 9, 1) }
        };
        return testCases;
    }

    [Theory]
    [MemberData(nameof(NextDateTestCases))]
    public void NextDate_ValidDateOnly_NewDateOnly(DateOnly originalDate, DateOnly effectiveDate,
        DateIntervalType intervalType, int frequency, DateOnly expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.NextDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    public static TheoryData<DateOnly, DateOnly, DateIntervalType, int, DateOnly> PreviousDateTestCases()
    {
        var testCases = new TheoryData<DateOnly, DateOnly, DateIntervalType, int, DateOnly>
        {
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1, new DateOnly(2014, 1, 9) },
            { new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1, new DateOnly(2015, 9, 24) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1, new DateOnly(2015, 4, 2) },
            { new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1, new DateOnly(2015, 9, 24) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 2, new DateOnly(2015, 4, 2) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 5, new DateOnly(2015, 3, 5) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 1, new DateOnly(2015, 3, 9) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 1, 15), DateIntervalType.Monthly, 1, new DateOnly(2014, 12, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 2, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 1, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 3, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 2, 28) },
            { new DateOnly(2014, 1, 31), new DateOnly(2016, 3, 15), DateIntervalType.Monthly, 1, new DateOnly(2016, 2, 29) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 4, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 3, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 5, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 4, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 6, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 5, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 7, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 6, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 8, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 7, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 9, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 8, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 10, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 9, 30) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 11, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 10, 31) },
            { new DateOnly(2014, 1, 31), new DateOnly(2015, 12, 15), DateIntervalType.Monthly, 1, new DateOnly(2015, 11, 30) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 2, new DateOnly(2015, 3, 9) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 1, new DateOnly(2015, 1, 9) },
            { new DateOnly(2014, 2, 28), new DateOnly(2017, 2, 1), DateIntervalType.Yearly, 1, new DateOnly(2016, 2, 28) },
            { new DateOnly(2014, 1, 9), new DateOnly(2022, 4, 6), DateIntervalType.Yearly, 7, new DateOnly(2021, 1, 9) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Daily, 3, new DateOnly(2015, 4, 4) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Daily, 300, new DateOnly(2014, 11, 5) }
        };
        return testCases;
    }

    [Theory]
    [MemberData(nameof(PreviousDateTestCases))]
    public void PreviousDate_ValidDateOnly_NewDateOnly(DateOnly originalDate, DateOnly effectiveDate,
        DateIntervalType intervalType, int frequency, DateOnly expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.PreviousDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    public static TheoryData<DateOnly, DateOnly, DateIntervalType, int, (DateOnly From, DateOnly To)> IntervalTestCases()
    {
        var testCases = new TheoryData<DateOnly, DateOnly, DateIntervalType, int, (DateOnly From, DateOnly To)>
        {
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Weekly, 2, (From: new DateOnly(2015, 4, 2), To: new DateOnly(2015, 4, 15)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Weekly, 5, (From: new DateOnly(2015, 3, 5), To: new DateOnly(2015, 4, 8)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Monthly, 1, (From: new DateOnly(2015, 3, 9), To: new DateOnly(2015, 4, 8)) },
            { new DateOnly(2014, 1, 1), new DateOnly(2016, 2, 15), DateIntervalType.Monthly, 2, (From: new DateOnly(2016, 1, 1), To: new DateOnly(2016, 2, 29)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Yearly, 1, (From: new DateOnly(2015, 1, 9), To: new DateOnly(2016, 1, 8)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2012, 4, 7), DateIntervalType.Yearly, 1, (From: new DateOnly(2012, 1, 9), To: new DateOnly(2013, 1, 8)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Daily, 3, (From: new DateOnly(2015, 4, 7), To: new DateOnly(2015, 4, 9)) },
            { new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Daily, 300, (From: new DateOnly(2014, 11, 5), To: new DateOnly(2015, 8, 31)) }
        };
        return testCases;
    }

    [Theory]
    [MemberData(nameof(IntervalTestCases))]
    public void TestInterval(DateOnly originalDate, DateOnly effectiveDate, DateIntervalType intervalType, int frequency, (DateOnly From, DateOnly To) expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.Interval(originalDate, effectiveDate, frequency).Should().BeEquivalentTo(expected);
    }

    public static TheoryData<DateIntervalType, int, DateOnly, DateOnly, DateOnly, DateOnly[]> DatesInRangeTestCases()
    {
        var testCases = new TheoryData<DateIntervalType, int, DateOnly, DateOnly, DateOnly, DateOnly[]>
        {
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 3, 1), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 1), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 1, 1), [] },
            { DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 1), new DateOnly(2014, 4, 1), Array.Empty<DateOnly>() },
            { DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 3, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 9), new DateOnly(2014, 2, 16), new DateOnly(2014, 2, 23) } },
            { DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 9), new DateOnly(2014, 2, 16), new DateOnly(2014, 2, 23) } },
            { DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 1), Array.Empty<DateOnly>() },
            { DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 4, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 2) } },
            { DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 4, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 2) } },
            { DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 1), Array.Empty<DateOnly>() },
            { DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2016, 4, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2015, 2, 2), new DateOnly(2016, 2, 2) } },
            { DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2016, 4, 1), new[] { new DateOnly(2014, 2, 2), new DateOnly(2015, 2, 2), new DateOnly(2016, 2, 2) } },
            { DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2013, 1, 1), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2013, 1, 1), new DateOnly(2014, 2, 1), Array.Empty<DateOnly>() },
            { DateIntervalType.Daily, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1), new DateOnly(2014, 2, 10), new[] { new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 3), new DateOnly(2014, 2, 4), new DateOnly(2014, 2, 5), new DateOnly(2014, 2, 6), new DateOnly(2014, 2, 7), new DateOnly(2014, 2, 8), new DateOnly(2014, 2, 9), new DateOnly(2014, 2, 10) } },
            { DateIntervalType.Daily, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2), new[] { new DateOnly(2014, 2, 2) } },
            { DateIntervalType.Daily, 1, new DateOnly(2014, 2, 2), new DateOnly(2013, 1, 1), new DateOnly(2014, 2, 1), Array.Empty<DateOnly>() },
        };
        return testCases;
    }

    [Theory]
    [MemberData(nameof(DatesInRangeTestCases))]
    public void GivenDatesInRange_WhenValidDateOnly_ThenListOfDatesInRange(DateIntervalType intervalType, int frequency, DateOnly effective, DateOnly from, DateOnly to, DateOnly[] expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.DatesInRange(effective, from, to, frequency).Should().Equal(expected);
    }

    public static TheoryData<DateOnly, DateOnly, DateIntervalType, int> GetIntervalTypeFromRangeTestCases()
    {
        var testCases = new TheoryData<DateOnly, DateOnly, DateIntervalType, int>
        {
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31), DateIntervalType.Yearly, 1 },
            { new DateOnly(2024, 2, 29), new DateOnly(2026, 2, 27), DateIntervalType.Yearly, 2 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), DateIntervalType.Monthly, 1 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 6, 30), DateIntervalType.Monthly, 6 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 7), DateIntervalType.Weekly, 1 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 21), DateIntervalType.Weekly, 3 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 1), DateIntervalType.Daily, 1 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 2), DateIntervalType.Daily, 2 },
            { new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 17), DateIntervalType.Daily, 17 },
        };
        return testCases;
    }

    [Theory]
    [MemberData(nameof(GetIntervalTypeFromRangeTestCases))]
    public void GivenGetIntervalTypeFromRange_WhenValidDateRange_ThenReturnIntervalTypeWithFrequency(DateOnly from, DateOnly to, DateIntervalType intervalType, int frequency) =>
        DateIntervalType.GetIntervalTypeFromRange(from, to).Should().Be((intervalType, frequency));
}
