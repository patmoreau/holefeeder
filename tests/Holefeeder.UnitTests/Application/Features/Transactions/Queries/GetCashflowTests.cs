using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Queries.GetCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

[UnitTest, Category("Application")]
public class GetCashflowTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request((CashflowId)faker.RandomGuid()));

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, CashflowId.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
