using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.ServiceTests.Shared.Core.Search;

public class OffsetTests
{
    [Fact]
    public void Create_Succeeds_WhenValid()
    {
        var result = Offset.Create(0);

        result.Should().BeSuccessful().And.WithValue(Offset.Valid(0));
    }

    [Fact]
    public void Create_Fails_WhenValueIsInvalid()
    {
        var result = Offset.Create(-1);

        result.Should().BeFailure().And.WithError(OffsetErrors.Invalid);
    }

    [Fact]
    public void Create_ReturnsDefault_WhenTextIsMissing()
    {
        var result = Offset.Create(null);

        result.Should().BeSuccessful().And.WithValue(Offset.Default);
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("25", 25)]
    [InlineData(" 42 ", 42)]
    [InlineData("+7", 7)]
    public void Create_Succeeds_WhenTextIsAValidNumber(string value, int expected)
    {
        var result = Offset.Create(value);

        result.Should().BeSuccessful().And.WithValue(Offset.Valid(expected));
    }

    [Theory]
    [InlineData("-1")]
    public void Create_Fails_WhenTextIsANumberOutOfRange(string value)
    {
        var result = Offset.Create(value);

        result.Should().BeFailure().And.WithError(OffsetErrors.Invalid);
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
        var result = Offset.Create(value);

        result.Should().BeFailure().And.WithError(OffsetErrors.Invalid);
    }
}
