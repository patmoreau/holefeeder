using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

public class ModifyAccountTests
{
    private readonly Faker<Request> _faker;

    public ModifyAccountTests() => _faker = new AutoFaker<Request>();

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
