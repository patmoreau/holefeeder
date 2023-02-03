using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class CreateStoreItemRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public static CreateStoreItemRequestBuilder GivenACreateStoreItemRequest()
    {
        return new();
    }

    public CreateStoreItemRequestBuilder WithCode(string code)
    {
        _faker.RuleFor(x => x.Code, code);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoCode()
    {
        _faker.RuleFor(x => x.Code, string.Empty);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoData()
    {
        _faker.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
