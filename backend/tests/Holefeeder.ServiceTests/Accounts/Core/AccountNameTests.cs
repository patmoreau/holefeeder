using Holefeeder.Service.Accounts.Core;

namespace Holefeeder.ServiceTests.Accounts.Core;

public class AccountNameTests
{
    [Fact]
    public void Create_Succeeds_WhenTheNameHasAHundredCharacters()
    {
        var value = new string('a', 100);

        AccountName.Create(value).Should().BeSuccessful().And.WithValue(AccountName.Valid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Fails_WhenTheNameIsEmpty(string? value) =>
        AccountName.Create(value).Should().BeFailure().And.WithError(AccountNameErrors.Invalid);

    [Fact]
    public void Create_Fails_WhenTheNameIsTooLong() =>
        AccountName.Create(new string('a', 101)).Should().BeFailure().And.WithError(AccountNameErrors.Invalid);

    [Fact]
    public void Invalid_DescribesTheLengthRule()
    {
        AccountNameErrors.Invalid.Code.Should().Be("account-name-invalid");
        AccountNameErrors.Invalid.Description.Should().Be("Account name must be between 1 and 100 characters.");
    }

    [Fact]
    public void AlreadyExists_DescribesTheUniquenessRule()
    {
        AccountNameErrors.AlreadyExists.Code.Should().Be("account-name-already-exists");
        AccountNameErrors.AlreadyExists.Description.Should().Be("Account name already exists.");
    }
}
