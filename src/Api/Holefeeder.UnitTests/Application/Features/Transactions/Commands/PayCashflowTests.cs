using System.Threading.Tasks;

using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class PayCashflowTests
{
    private readonly AutoFaker<Request> _faker = new();

    public PayCashflowTests()
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
    public async Task GivenValidator_WhenCashflowIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.CashflowId, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CashflowId);
    }

    [Fact]
    public async Task GivenValidator_WhenCashflowDateIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.CashflowDate, _ => default).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CashflowDate);
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
