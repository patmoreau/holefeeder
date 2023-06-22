using System.Threading.Tasks;
using Holefeeder.Application.Features.StoreItems.Queries;
using Microsoft.AspNetCore.Http;
using static Holefeeder.Application.Features.StoreItems.Queries.GetStoreItems;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Queries;

[UnitTest]
public class GetStoreItemsTests
{
    private readonly Faker<Request> _faker = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Number(), faker.Random.Int(1), Array.Empty<string>(),
            Array.Empty<string>()))
        .RuleFor(x => x.Offset, faker => faker.Random.Number())
        .RuleFor(x => x.Limit, faker => faker.Random.Int(1))
        .RuleFor(x => x.Sort, Array.Empty<string>())
        .RuleFor(x => x.Filter, Array.Empty<string>());

    public GetStoreItemsTests()
    {
        int countDummy = Fakerizer.Random.Number(100);
        new Faker<StoreItemViewModel>()
            .CustomInstantiator(faker =>
                new StoreItemViewModel(faker.Random.Guid(), faker.Random.Hash(), faker.Lorem.Paragraph()))
            .RuleFor(x => x.Id, faker => faker.Random.Guid())
            .RuleFor(x => x.Code, faker => faker.Random.Hash())
            .RuleFor(x => x.Data, faker => faker.Lorem.Paragraph())
            .Generate(countDummy);
    }

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
