using static Holefeeder.Application.Features.Transactions.Queries.GetTransaction;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

public class GetTransactionTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid()));

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
