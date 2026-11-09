using Bogus;

using Holefeeder.Service.StoreItems.Core;

namespace Holefeeder.ServiceTests.StoreItems.Core;

public class CodeTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Create_Succeeds_WhenValueIsValid()
    {
        var value = _faker.Random.AlphaNumeric(10);
        var code = Code.Create(value);

        code.Should().BeSuccessful().And.WithValue(Code.Valid(value));
    }

    [Fact]
    public void Create_Fails_WhenValueTooLong()
    {
        var value = _faker.Random.AlphaNumeric(101);
        var code = Code.Create(value);

        code.Should().BeFailure().And.WithError(CodeErrors.Invalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_Fails_WhenValueIsEmpty(string? value)
    {
        var code = Code.Create(value);

        code.Should().BeFailure().And.WithError(CodeErrors.Invalid);
    }

    [Fact]
    public void Invalid_DescribesTheLengthRule()
    {
        CodeErrors.Invalid.Code.Should().Be("code-invalid");
        CodeErrors.Invalid.Description.Should().Be("Code must be between 1 and 100 characters.");
    }
}
