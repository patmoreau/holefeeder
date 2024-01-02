using static Holefeeder.Application.Features.Accounts.Queries.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

[UnitTest]
public class GetAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid()));

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
