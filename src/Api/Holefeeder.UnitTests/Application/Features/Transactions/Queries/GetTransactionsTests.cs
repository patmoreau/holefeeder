using Microsoft.AspNetCore.Http;

using static Holefeeder.Application.Features.Transactions.Queries.GetTransactions;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

[UnitTest]
public class GetTransactionsTests
{
    private readonly Faker<Request> _faker;

    public GetTransactionsTests() =>
        _faker = new Faker<Request>()
            .CustomInstantiator(faker => new Request(faker.Random.Number(), faker.Random.Int(1), Array.Empty<string>(),
                Array.Empty<string>()))
            .RuleFor(x => x.Offset, faker => faker.Random.Number())
            .RuleFor(x => x.Limit, faker => faker.Random.Int(1))
            .RuleFor(x => x.Sort, Array.Empty<string>())
            .RuleFor(x => x.Filter, Array.Empty<string>());

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        var httpContext = new DefaultHttpContext
        {
            Request = { QueryString = new QueryString("?offset=10&limit=100&sort=data&filter=code:eq:settings") }
        };

        // act
        var result = await Request.BindAsync(httpContext, null!);

        // assert
        string[] sort = ["data"];
        string[] filter = ["code:eq:settings"];
        result.Should().BeEquivalentTo(new Request(10, 100, sort, filter));
    }

    [Fact]
    public void GivenValidator_WhenOffsetIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Offset, -1).Generate();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Offset);
    }

    [Fact]
    public void GivenValidator_WhenLimitIsInvalid_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Limit, 0).Generate();

        Validator validator = new();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Limit);
    }
}
