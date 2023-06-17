using System.Threading.Tasks;
using Holefeeder.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

public class GetUpcomingTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Date.Past().Date, faker.Date.Future().Date));

    [Fact]
    public async Task GivenRequest_WhenBindingFromHttpContext_ThenReturnRequest()
    {
        // arrange
        Request? request = _faker.Generate();
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            Request =
            {
                QueryString =
                    new QueryString($"?from={request.From.ToPersistent()}&to={request.To.ToPersistent()}")
            }
        };

        // act
        Request? result = await Request.BindAsync(httpContext, null!);

        // assert
        result.Should().BeEquivalentTo(new Request(request.From, request.To));
    }

    [Fact]
    public void GivenValidator_WhenFromIsNull_ThenError()
    {
        // arrange
        Request? request = _faker.RuleFor(x => x.From, _ => default).Generate();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.From);
    }
}
