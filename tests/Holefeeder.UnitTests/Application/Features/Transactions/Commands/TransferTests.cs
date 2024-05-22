using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.Transfer;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest]
public class TransferTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Date.SoonDateOnly(), faker.Finance.Amount(), faker.Lorem.Paragraphs(),
            faker.RandomGuid(), faker.RandomGuid()))
        .RuleFor(x => x.Date, faker => faker.Date.SoonDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Paragraphs())
        .RuleFor(x => x.FromAccountId, faker => faker.RandomGuid())
        .RuleFor(x => x.ToAccountId, faker => faker.RandomGuid())
        ;

    public TransferTests() => _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(1M));

    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Date, DateOnly.MinValue).Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountNotGreaterThanZero_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Amount, faker => faker.Random.Decimal(decimal.MinValue, decimal.Zero))
            .Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

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
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

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
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

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
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
