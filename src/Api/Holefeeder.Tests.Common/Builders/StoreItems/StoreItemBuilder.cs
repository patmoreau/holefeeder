using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class StoreItemBuilder : IBuilder<StoreItem>
{
    private StoreItem _entity;

    private StoreItemBuilder() => _entity = new StoreItemFactory().Generate();

    public StoreItem Build() => _entity;

    public static StoreItemBuilder GivenAStoreItem() => new StoreItemBuilder();

    public StoreItemBuilder WithId(Guid id)
    {
        _entity = _entity with { Id = id };
        return this;
    }

    public StoreItemBuilder WithNoId()
    {
        _entity = _entity with { Id = Guid.Empty };
        return this;
    }

    public StoreItemBuilder WithCode(string code)
    {
        _entity = _entity with { Code = code };
        return this;
    }

    public StoreItemBuilder ForUser(Guid userId)
    {
        _entity = _entity with { UserId = userId };
        return this;
    }

    public StoreItemBuilder ForNoUser()
    {
        _entity = _entity with { UserId = Guid.Empty };
        return this;
    }
}
