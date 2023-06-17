using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

public class ModifyAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker =>
            new Request(faker.Random.Guid(), faker.Lorem.Word(), faker.Finance.Amount(), faker.Lorem.Sentence()));

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenValidationError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Id, _ => Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
