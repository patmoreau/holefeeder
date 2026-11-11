using Holefeeder.Service.Cashflows.Core;

namespace Holefeeder.ServiceTests.Cashflows.Core;

public class MoneyTests
{
    [Theory]
    [InlineData("0", 0)]
    [InlineData("1450", 1450)]
    [InlineData("88.40", 88.40)]
    public void Create_Succeeds_WhenValueIsAnAmount(string value, decimal amount) =>
        Money.Create(value).Should().BeSuccessful().And.WithValue(Money.Valid(amount));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("twelve")]
    [InlineData("-1")]
    [InlineData("1,450.00")]
    [InlineData("1.005")]
    public void Create_Fails_WhenValueIsNotAPositiveAmountInCents(string? value) =>
        Money.Create(value).Should().BeFailure().And.WithError(MoneyErrors.Invalid);

    [Fact]
    public void Invalid_DescribesTheRule()
    {
        MoneyErrors.Invalid.Code.Should().Be("amount-invalid");
        MoneyErrors.Invalid.Description.Should().Be("Amount must be zero or more, with at most two decimals.");
    }
}
