using System.Threading.Tasks;
using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class MakePurchaseTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .RuleFor(x => x.Date, faker => faker.Date.Recent())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.CategoryId, faker => faker.Random.Guid());

    public MakePurchaseTests() => _faker.RuleFor(x => x.Cashflow, _ => null);

    [Fact]
    public async Task GivenValidator_WhenAccountIdIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.AccountId, Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.AccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenCategoryIdIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.CategoryId, Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CategoryId);
    }

    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Date, _ => default).Generate();

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
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        Request? request = _faker.Generate();

        Validator validator = new();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
