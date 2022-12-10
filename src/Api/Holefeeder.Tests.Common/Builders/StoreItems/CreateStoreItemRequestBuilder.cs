using Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class CreateStoreItemRequestBuilder : IBuilder<Request>
{
    private Request _request;

    private CreateStoreItemRequestBuilder()
    {
        _request = new CreateStoreItemRequestFactory().Generate();
    }

    public Request Build()
    {
        return _request;
    }

    public static CreateStoreItemRequestBuilder GivenACreateStoreItemRequest()
    {
        return new();
    }

    public CreateStoreItemRequestBuilder WithCode(string code)
    {
        _request = _request with {Code = code};
        return this;
    }
}
