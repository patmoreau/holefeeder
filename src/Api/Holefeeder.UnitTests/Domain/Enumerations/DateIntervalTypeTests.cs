using System.Collections.Generic;

using Holefeeder.Domain.Enumerations;

namespace Holefeeder.UnitTests.Domain.Enumerations;

[UnitTest]
public class DateIntervalTypeTests
{
    public static IEnumerable<object[]> NextDateTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateOnly(2014, 1, 9)
            };
            yield return new object[]
            {
                new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateOnly(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateOnly(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateOnly(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 2,
                new DateOnly(2015, 4, 16)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 5,
                new DateOnly(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 4, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 1, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 1, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 2, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 2, 28)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2016, 2, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2016, 2, 29)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 3, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 3, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 4, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 4, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 5, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 5, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 6, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 6, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 7, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 7, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 8, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 8, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 9, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 9, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 10, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 10, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 11, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 11, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 12, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 12, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 2,
                new DateOnly(2015, 5, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 1,
                new DateOnly(2016, 1, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 2, 28), new DateOnly(2016, 2, 1), DateIntervalType.Yearly, 1,
                new DateOnly(2016, 2, 28)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 7,
                new DateOnly(2021, 1, 9)
            };
        }
    }

    public static IEnumerable<object[]> PreviousDateTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateOnly(2014, 1, 9)
            };
            yield return new object[]
            {
                new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.OneTime, 1,
                new DateOnly(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateOnly(2015, 4, 2)
            };
            yield return new object[]
            {
                new DateOnly(2015, 9, 24), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 1,
                new DateOnly(2015, 9, 24)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 2,
                new DateOnly(2015, 4, 2)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Weekly, 5,
                new DateOnly(2015, 3, 5)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 3, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 1, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2014, 12, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 2, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 1, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 3, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 2, 28)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2016, 3, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2016, 2, 29)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 4, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 3, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 5, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 4, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 6, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 5, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 7, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 6, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 8, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 7, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 9, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 8, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 10, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 9, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 11, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 10, 31)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 31), new DateOnly(2015, 12, 15), DateIntervalType.Monthly, 1,
                new DateOnly(2015, 11, 30)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Monthly, 2,
                new DateOnly(2015, 3, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 6), DateIntervalType.Yearly, 1,
                new DateOnly(2015, 1, 9)
            };
            yield return new object[]
            {
                new DateOnly(2014, 2, 28), new DateOnly(2017, 2, 1), DateIntervalType.Yearly, 1,
                new DateOnly(2016, 2, 28)
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2022, 4, 6), DateIntervalType.Yearly, 7,
                new DateOnly(2021, 1, 9)
            };
        }
    }

    public static IEnumerable<object[]> IntervalTestCases
    {
        get
        {
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Weekly, 2,
                (From: new DateOnly(2015, 4, 2), To: new DateOnly(2015, 4, 15))
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Weekly, 5,
                (From: new DateOnly(2015, 3, 5), To: new DateOnly(2015, 4, 8))
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Monthly, 1,
                (From: new DateOnly(2015, 3, 9), To: new DateOnly(2015, 4, 8))
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 1), new DateOnly(2016, 2, 15), DateIntervalType.Monthly, 2,
                (From: new DateOnly(2016, 1, 1), To: new DateOnly(2016, 2, 29))
            };
            yield return new object[]
            {
                new DateOnly(2014, 1, 9), new DateOnly(2015, 4, 7), DateIntervalType.Yearly, 1,
                (From: new DateOnly(2015, 1, 9), To: new DateOnly(2016, 1, 8))
            };
        }
    }

    public static IEnumerable<object[]> DatesInRangeTestCases
    {
        get
        {
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 3, 1), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 3, 1), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 1, 1), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                DateIntervalType.OneTime, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 1),
                new DateOnly(2014, 4, 1), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 3, 1),
                new[]
                {
                    new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 9), new DateOnly(2014, 2, 16),
                    new DateOnly(2014, 2, 23)
                }
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 3, 1),
                new[]
                {
                    new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 9), new DateOnly(2014, 2, 16),
                    new DateOnly(2014, 2, 23)
                }
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Weekly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 2, 1), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 4, 1), new[] {new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 4, 1), new[] {new DateOnly(2014, 2, 2), new DateOnly(2014, 3, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Monthly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2014, 2, 1), Array.Empty<DateOnly>()
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 1, 1),
                new DateOnly(2016, 4, 1),
                new[] {new DateOnly(2014, 2, 2), new DateOnly(2015, 2, 2), new DateOnly(2016, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2014, 2, 2),
                new DateOnly(2016, 4, 1),
                new[] {new DateOnly(2014, 2, 2), new DateOnly(2015, 2, 2), new DateOnly(2016, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2013, 1, 1),
                new DateOnly(2014, 2, 2), new[] {new DateOnly(2014, 2, 2)}
            };
            yield return new object[]
            {
                DateIntervalType.Yearly, 1, new DateOnly(2014, 2, 2), new DateOnly(2013, 1, 1),
                new DateOnly(2014, 2, 1), Array.Empty<DateOnly>()
            };
        }
    }

    [Theory]
    [MemberData(nameof(NextDateTestCases))]
    public void NextDate_ValidDateOnly_NewDateOnly(DateOnly originalDate, DateOnly effectiveDate,
        DateIntervalType intervalType, int frequency, DateOnly expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.NextDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(PreviousDateTestCases))]
    public void PreviousDate_ValidDateOnly_NewDateOnly(DateOnly originalDate, DateOnly effectiveDate,
        DateIntervalType intervalType, int frequency, DateOnly expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.PreviousDate(originalDate, effectiveDate, frequency).Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IntervalTestCases))]
    public void TestInterval(DateOnly originalDate, DateOnly effectiveDate, DateIntervalType intervalType,
        int frequency, (DateOnly From, DateOnly To) expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.Interval(originalDate, effectiveDate, frequency).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(DatesInRangeTestCases))]
    public void GivenDatesInRange_WhenValidDateOnly_ThenListOfDatesInRange(DateIntervalType intervalType,
        int frequency, DateOnly effective, DateOnly from, DateOnly to, DateOnly[] expected)
    {
        ArgumentNullException.ThrowIfNull(intervalType);

        intervalType.DatesInRange(effective, from, to, frequency).Should().Equal(expected);
    }
}
