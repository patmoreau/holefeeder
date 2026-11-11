using Holefeeder.Service.Accounts.Api;

namespace Holefeeder.ServiceTests.Accounts.Api;

public class OpenDateTests
{
    [Fact]
    public void Parse_ReadsAnIsoDate() =>
        OpenDate.Parse("2025-03-01").Should().BeSuccessful().And.WithValue(new DateOnly(2025, 3, 1));

    [Theory]
    [InlineData(null)]
    [InlineData("0001-01-01")]
    [InlineData("03/01/2025")]
    public void Parse_Fails_WhenTheDateIsMissing(string? value) =>
        OpenDate.Parse(value).Should().BeFailure().And.WithError(OpenDateErrors.Invalid);

    [Fact]
    public void Invalid_DescribesTheDateRule()
    {
        OpenDateErrors.Invalid.Code.Should().Be("open-date-invalid");
        OpenDateErrors.Invalid.Description.Should().Be("Open date must be a date as YYYY-MM-DD.");
    }
}
