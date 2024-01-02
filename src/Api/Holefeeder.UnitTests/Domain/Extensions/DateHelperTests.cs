using System.Collections.Generic;

using Holefeeder.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Extensions;

[UnitTest]
public class DateHelperTests
{
    public static IEnumerable<object[]> ParsePersistentTestCases
    {
        get
        {
            yield return new object[] { "2014-01-09", new DateOnly(2014, 1, 9) };
            yield return new object[] { "2016-12-28", new DateOnly(2016, 12, 28) };
        }
    }

    [Theory]
    [MemberData(nameof(ToPersistentTestCases))]
    public void ToPersistent_DateOnlyValue_ReturnsString(DateOnly dateOnly, string expected) =>
        dateOnly.ToPersistent().Should().Be(expected);

    public static IEnumerable<object[]> ToPersistentTestCases()
    {
        yield return new object[] { new DateOnly(2014, 1, 9), "2014-01-09" };
        yield return new object[] { new DateOnly(2016, 12, 28), "2016-12-28" };
    }

    [Theory]
    [MemberData(nameof(ParsePersistentTestCases))]
    public void ParsePersistent_StringValue_ReturnsDateOnly(string dateOnly, DateOnly expected) =>
        dateOnly.ParsePersistent().Should().Be(expected);
}
