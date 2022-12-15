using System.Threading.Tasks;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class TransferTests
{
    private readonly AutoFaker<Request> _faker = new();

    public TransferTests()
    {
        _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(1M));
    }

    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Date, DateTime.MinValue).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountNotGreaterThanZero_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Amount, faker => faker.Random.Decimal(Decimal.MinValue, Decimal.Zero))
            .Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public async Task GivenValidator_WhenFromAccountIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.FromAccountId, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.FromAccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenToAccountIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.ToAccountId, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.ToAccountId);
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
