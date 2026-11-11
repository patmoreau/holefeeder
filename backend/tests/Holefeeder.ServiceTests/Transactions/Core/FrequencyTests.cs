using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.ServiceTests.Transactions.Core;

public class FrequencyTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("12", 12)]
    public void Create_Succeeds_WhenValueIsOneOrMore(string value, int frequency) =>
        Frequency.Create(value).Should().BeSuccessful().And.WithValue(Frequency.Valid(frequency));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("monthly")]
    public void Create_Fails_WhenValueIsNotAWholeNumberOfOneOrMore(string? value) =>
        Frequency.Create(value).Should().BeFailure().And.WithError(FrequencyErrors.Invalid);
}
