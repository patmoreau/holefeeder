using static Holefeeder.Application.Features.Accounts.Queries.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

public class GetAccountTests
{
    private readonly AutoFaker<Request> _faker = new();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
