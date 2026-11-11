using Holefeeder.Service.Shared.Core;


namespace Holefeeder.ServiceTests.Shared.Core;

public class AccountTypeTests
{
    [Theory]
    [InlineData("Checking", 1, true)]
    [InlineData("savings", 1, true)]
    [InlineData("Investment", 1, true)]
    [InlineData("CreditCard", -1, false)]
    [InlineData("creditline", -1, false)]
    [InlineData("Loan", -1, false)]
    [InlineData("Mortgage", -1, false)]
    public void Create_KnowsTheSignOfEachType(string name, int sign, bool isAsset)
    {
        var result = AccountType.Create(name);

        result.Should().BeSuccessful();
        result.Value.Sign.Should().Be(sign);
        result.Value.IsAsset.Should().Be(isAsset);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Wallet")]
    public void Create_Fails_WhenTypeIsUnknown(string? name)
    {
        var result = AccountType.Create(name);

        result.Should().BeFailure().And.WithError(AccountTypeErrors.Invalid);
    }
}
