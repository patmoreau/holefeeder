using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static Holefeeder.Application.Features.Transactions.Queries.GetTransactions;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

public class GetTransactionsTests
{
    private readonly Faker<Request> _faker;

    public GetTransactionsTests() =>
        _faker = new AutoFaker<Request>()
            .RuleFor(fake => fake.Offset, fake => fake.Random.Number())
            .RuleFor(fake => fake.Limit, fake => fake.Random.Int(1));

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            Request = { QueryString = new QueryString("?offset=10&limit=100&sort=data&filter=code:eq:settings") }
        };

        // act
        Request? result = await Request.BindAsync(httpContext, null!);

        // assert
        result.Should().BeEquivalentTo(new Request(10, 100, new[] { "data" }, new[] { "code:eq:settings" }));
    }

    [Fact]
    public void GivenValidator_WhenOffsetIsInvalid_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Offset, -1).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Offset);
    }

    [Fact]
    public void GivenValidator_WhenLimitIsInvalid_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.Limit, 0).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Limit);
    }
}
