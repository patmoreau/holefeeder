using Holefeeder.Service.Insights.Core.GetOverview;

namespace Holefeeder.ServiceTests.Insights.Core.GetOverview;

public class PeriodCountTests
{
    [Theory]
    [InlineData("3", 3)]
    [InlineData("6", 6)]
    [InlineData("12", 12)]
    [InlineData(" 12 ", 12)]
    public void Create_AcceptsTheOfferedCounts(string value, int expected)
    {
        var result = PeriodCount.Create(value);

        result.Should().BeSuccessful();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_DefaultsToSix_WhenNoCountIsGiven(string? value)
    {
        var result = PeriodCount.Create(value);

        result.Should().BeSuccessful();
        result.Value.Value.Should().Be(6);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("4")]
    [InlineData("24")]
    [InlineData("-3")]
    [InlineData("six")]
    public void Create_Fails_WhenTheCountIsNotOffered(string value)
    {
        var result = PeriodCount.Create(value);

        result.Should().BeFailure().And.WithError(PeriodCountErrors.Invalid);
    }
}
