using System.Threading.Tasks;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class ModifyCashflowTests
{
    private readonly AutoFaker<Request> _faker = new();

    public ModifyCashflowTests()
    {
        _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.One, decimal.MaxValue));
    }

    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

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
        var result = await validator.TestValidateAsync(request);

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
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
