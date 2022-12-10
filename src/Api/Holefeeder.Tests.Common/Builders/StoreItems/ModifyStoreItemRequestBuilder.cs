using Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class ModifyStoreItemRequestBuilder : IBuilder<Request>
{
    private Request _request;

    private ModifyStoreItemRequestBuilder()
    {
        _request = new ModifyStoreItemRequestFactory().Generate();
    }

    public Request Build()
    {
        return _request;
    }

    public static ModifyStoreItemRequestBuilder GivenAModifyStoreItemRequest()
    {
        return new();
    }

    public ModifyStoreItemRequestBuilder WithId(Guid id)
    {
        _request = _request with {Id = id};
        return this;
    }

    public ModifyStoreItemRequestBuilder WithData(string data)
    {
        _request = _request with {Data = data};
        return this;
    }
}
