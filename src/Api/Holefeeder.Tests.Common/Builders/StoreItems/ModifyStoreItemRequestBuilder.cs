using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class ModifyStoreItemRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public static ModifyStoreItemRequestBuilder GivenAModifyStoreItemRequest() => new();

    public ModifyStoreItemRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithData(string data)
    {
        _faker.RuleFor(x => x.Data, data);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoData()
    {
        _faker.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
