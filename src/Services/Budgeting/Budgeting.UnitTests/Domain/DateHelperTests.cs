using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Extensions;

using FluentAssertions;

using Xunit;

namespace DrifterApps.Holefeeder.Budgeting.UnitTests.Domain;

public class DateHelperTests
{
    public static IEnumerable<object[]> ParsePersistentTestCases
    {
        get
        {
            yield return new object[] {"2014-01-09", new DateTime(2014, 1, 9)};
            yield return new object[] {"2016-12-28", new DateTime(2016, 12, 28)};
        }
    }

    [Theory]
    [MemberData(nameof(ToPersistentTestCases))]
    public void ToPersistent_DateTimeValue_ReturnsString(DateTime dateTime, string expected)
    {
        dateTime.ToPersistent().Should().Be(expected);
    }

    public static IEnumerable<object[]> ToPersistentTestCases()
    {
        yield return new object[] {new DateTime(2014, 1, 9), "2014-01-09"};
        yield return new object[] {new DateTime(2016, 12, 28), "2016-12-28"};
    }

    [Theory]
    [MemberData(nameof(ParsePersistentTestCases))]
    public void ParsePersistent_StringValue_ReturnsDateTime(string dateTime, DateTime expected)
    {
        dateTime.ParsePersistent().Should().Be(expected);
    }
}
