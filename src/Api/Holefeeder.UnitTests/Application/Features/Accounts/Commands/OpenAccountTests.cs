using System.Threading.Tasks;
using Holefeeder.Domain.Features.Accounts;
using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest]
public class OpenAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.PickRandom<AccountType>(AccountType.List), faker.Lorem.Word(),
            faker.Date.RecentDateOnly(), faker.Finance.Amount(), faker.Lorem.Sentence()))
        .RuleFor(x => x.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.OpenDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence());

    [Fact]
    public async Task GivenValidator_WhenTypeIsNull_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Type, _ => null!).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GivenValidator_WhenNameIsInvalid_ThenError(string name)
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Name, _ => name).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenValidator_WhenOpenDateIsInvalid_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.OpenDate, _ => DateOnly.MinValue).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.OpenDate);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        Request? request = _faker.Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
