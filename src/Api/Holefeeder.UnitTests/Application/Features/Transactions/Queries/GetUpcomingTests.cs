using System.Threading.Tasks;

using Holefeeder.Domain.Extensions;

using Microsoft.AspNetCore.Http;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

public class GetUpcomingTests
{
    private readonly Faker<Request> _faker;

    public GetUpcomingTests()
    {
        _faker = new AutoFaker<Request>()
            .RuleFor(fake => fake.From, fake => fake.Date.Past().Date)
            .RuleFor(fake => fake.To, fake => fake.Date.Future().Date);
    }

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        var request = _faker.Generate();
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                QueryString =
                    new QueryString($"?from={request.From.ToPersistent()}&to={request.To.ToPersistent()}")
            }
        };

        // act
        var result = await Request.BindAsync(httpContext, null!);

        // assert
        result.Should().BeEquivalentTo(new Request(request.From, request.To));
    }

    [Fact]
    public void GivenValidator_WhenFromIsNull_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.From, _ => default).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.From);
    }
}
