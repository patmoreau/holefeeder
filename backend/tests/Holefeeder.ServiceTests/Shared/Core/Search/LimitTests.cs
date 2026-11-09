using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Shared.Core.Search;

public class LimitTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var result = Limit.Create(1);

        result.Should().BeSuccessful().And.WithValue(Limit.Valid(1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_Fails_WhenValueIsInvalid(int value)
    {
        var result = Limit.Create(value);

        result.Should().BeFailure().And.WithError(LimitErrors.Invalid);
    }

    [Fact]
    public void Create_ReturnsDefault_WhenTextIsMissing()
    {
        var result = Limit.Create(null);

        result.Should().BeSuccessful().And.WithValue(Limit.Default);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    [InlineData(" 42 ", 42)]
    [InlineData("+7", 7)]
    public void Create_Succeeds_WhenTextIsAValidNumber(string value, int expected)
    {
        var result = Limit.Create(value);

        result.Should().BeSuccessful().And.WithValue(Limit.Valid(expected));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("101")]
    [InlineData("-1")]
    public void Create_Fails_WhenTextIsANumberOutOfRange(string value)
    {
        var result = Limit.Create(value);

        result.Should().BeFailure().And.WithError(LimitErrors.Invalid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc")]
    [InlineData("1.5")]
    [InlineData("1,000")]
    [InlineData("0x10")]
    [InlineData("99999999999")]
    public void Create_Fails_WhenTextIsNotAnInteger(string value)
    {
        var result = Limit.Create(value);

        result.Should().BeFailure().And.WithError(LimitErrors.Invalid);
    }
}
