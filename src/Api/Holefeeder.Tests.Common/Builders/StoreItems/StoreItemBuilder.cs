using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class StoreItemBuilder : IBuilder<StoreItem>
{
    private StoreItem _entity;

    private StoreItemBuilder()
    {
        _entity = new StoreItemFactory().Generate();
    }

    public StoreItem Build()
    {
        return _entity;
    }

    public static StoreItemBuilder GivenAStoreItem()
    {
        return new();
    }

    public StoreItemBuilder WithId(Guid id)
    {
        _entity = _entity with {Id = id};
        return this;
    }

    public StoreItemBuilder WithCode(string code)
    {
        _entity = _entity with {Code = code};
        return this;
    }

    public StoreItemBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
