using Microsoft.AspNetCore.Http;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccounts;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

[UnitTest, Category("Application")]
public class GetAccountsTests
{
    private readonly Faker<Request> _faker;

    public GetAccountsTests() =>
        _faker = new Faker<Request>()
            .CustomInstantiator(faker => new Request(faker.Random.Number(), faker.Random.Int(1), Array.Empty<string>(),
                Array.Empty<string>()))
            .RuleFor(fake => fake.Offset, fake => fake.Random.Number())
            .RuleFor(fake => fake.Limit, fake => fake.Random.Int(1))
            .RuleFor(fake => fake.Sort, Array.Empty<string>())
            .RuleFor(fake => fake.Filter, Array.Empty<string>());

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

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Limit);
    }
}
