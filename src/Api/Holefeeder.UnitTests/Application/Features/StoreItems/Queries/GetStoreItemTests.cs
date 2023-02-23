using static Holefeeder.Application.Features.StoreItems.Queries.GetStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Queries;

public class GetStoreItemTests
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
