using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest]
public class ModifyAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker =>
            new Request(faker.RandomGuid(), faker.Lorem.Word(), faker.Finance.Amount(), faker.Lorem.Sentence()))
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence());

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenValidationError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, _ => Guid.Empty).Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
