using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest]
public class FavoriteAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid(), faker.Random.Bool()))
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.IsFavorite, faker => faker.Random.Bool());

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
