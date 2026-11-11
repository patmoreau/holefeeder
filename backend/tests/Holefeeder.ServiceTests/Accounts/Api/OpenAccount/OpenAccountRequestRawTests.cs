using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Api.OpenAccount;
using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.OpenAccount;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Accounts.Api.OpenAccount;

public class OpenAccountRequestRawTests
{
    private static OpenAccountBody ABody(
        string? type = "creditCard", string? name = "Amex", string? openDate = "2025-03-01", string? openBalance = "250.75",
        string? description = "Travel card") =>
        new(type, name, openDate, openBalance, description);

    [Fact]
    public void ToOpenAccountRequest_ReturnsTheAccountToOpenForTheUser()
    {
        var user = AUser();

        var result = OpenAccountRequestRaw.ToOpenAccountRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(new OpenAccountRequest(
            user, AccountType.CreditCard, AccountName.Valid("Amex"), new DateOnly(2025, 3, 1), Money.Valid(250.75m), "Travel card"));
    }

    [Fact]
    public void ToOpenAccountRequest_DefaultsMissingOpenBalanceToZeroAndDescriptionToEmpty()
    {
        var result = OpenAccountRequestRaw.ToOpenAccountRequest(new(ABody(openBalance: null, description: null)), AUser());

        result.Should().BeSuccessful();
        result.Value.OpenBalance.Should().Be(Money.Zero);
        result.Value.Description.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0001-01-01")]
    [InlineData("03/01/2025")]
    public void ToOpenAccountRequest_Fails_WhenTheOpenDateIsMissing(string? openDate)
    {
        var result = OpenAccountRequestRaw.ToOpenAccountRequest(new(ABody(openDate: openDate)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            OpenAccountRequestErrors.CodeOpenDateInvalid);
    }

    [Fact]
    public void ToOpenAccountRequest_Fails_WithEveryInvalidField()
    {
        var result = OpenAccountRequestRaw.ToOpenAccountRequest(new(new OpenAccountBody(null, " ", null, "-1", null)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            AccountTypeErrors.CodeInvalid,
            AccountNameErrors.CodeInvalid,
            OpenAccountRequestErrors.CodeOpenDateInvalid,
            MoneyErrors.CodeInvalid);
    }

    [Fact]
    public void OpenDateInvalid_DescribesTheDateRule()
    {
        OpenAccountRequestErrors.OpenDateInvalid.Code.Should().Be("open-date-invalid");
        OpenAccountRequestErrors.OpenDateInvalid.Description.Should().Be("Open date must be a date as YYYY-MM-DD.");
    }
}
