using System.Threading.Tasks;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest]
public class ModifyCashflowTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, Array.Empty<string>());

    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountIsNotGreaterThanZero_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.MinValue, decimal.Zero))
            .Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
