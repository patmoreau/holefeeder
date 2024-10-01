using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Builders;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest, Category("Application")]
public class ModifyCashflowTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .RuleFor(x => x.Id, faker => (CashflowId)faker.RandomGuid())
        .RuleFor(x => x.Amount, MoneyBuilder.Create().Build())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, Array.Empty<string>());

    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, CashflowId.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenValidator_WhenEffectiveDateIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.EffectiveDate, () => default).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.EffectiveDate);
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
