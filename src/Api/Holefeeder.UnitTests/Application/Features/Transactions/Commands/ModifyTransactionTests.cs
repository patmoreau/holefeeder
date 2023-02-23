using System.Threading.Tasks;
using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class ModifyTransactionTests
{
    private readonly AutoFaker<Request> _faker = new();

    public ModifyTransactionTests() =>
        _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.One, decimal.MaxValue));

    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

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
        Request? request = _faker.RuleFor(x => x.Date, DateTime.MinValue).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountIsNotGreaterThanZero_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.MinValue, decimal.Zero))
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

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
