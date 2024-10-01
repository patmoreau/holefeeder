using Holefeeder.Domain.Extensions;

namespace Holefeeder.UnitTests.Domain.Extensions;

[UnitTest, Category("Domain")]
public class DateHelperTests
{
    private sealed class ParsePersistentData : TheoryData<string, DateOnly>
    {
        public ParsePersistentData()
        {
            Add("2014-01-09", new DateOnly(2014, 1, 9));
            Add("2016-12-28", new DateOnly(2016, 12, 28));
        }
    }

    [Theory]
    [ClassData(typeof(ToPersistentData))]
    public void ToPersistent_DateOnlyValue_ReturnsString(DateOnly dateOnly, string expected) =>
        dateOnly.ToPersistent().Should().Be(expected);

    private sealed class ToPersistentData : TheoryData<DateOnly, string>
    {
        public ToPersistentData()
        {
            Add(new DateOnly(2014, 1, 9), "2014-01-09");
            Add(new DateOnly(2016, 12, 28), "2016-12-28");
        }
    }

    [Theory]
    [ClassData(typeof(ParsePersistentData))]
    public void ParsePersistent_StringValue_ReturnsDateOnly(string dateOnly, DateOnly expected) =>
        dateOnly.ParsePersistent().Should().Be(expected);
}
