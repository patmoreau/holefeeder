using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

[UnitTest, Category("Application")]
public class GetAccountTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request((AccountId)faker.RandomGuid()));

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, AccountId.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
