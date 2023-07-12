using System.Threading.Tasks;
using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest]
public class PayCashflowTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .RuleFor(x => x.Date, faker => faker.Date.SoonDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.CashflowId, faker => faker.Random.Guid())
        .RuleFor(x => x.CashflowDate, faker => faker.Date.RecentDateOnly());

    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Date, DateOnly.MinValue).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountNotGreaterThanZero_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Amount, faker => faker.Random.Decimal(decimal.MinValue, decimal.Zero))
            .Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public async Task GivenValidator_WhenCashflowIdIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.CashflowId, Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CashflowId);
    }

    [Fact]
    public async Task GivenValidator_WhenCashflowDateIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.CashflowDate, _ => default).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CashflowDate);
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
