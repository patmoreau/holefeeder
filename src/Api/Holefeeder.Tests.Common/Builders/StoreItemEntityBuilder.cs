using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class StoreItemEntityBuilder : IBuilder<StoreItemEntity>
{
    private StoreItemEntity _entity;

    private StoreItemEntityBuilder()
    {
        _entity = new StoreItemEntityFactory().Generate();
    }

    public StoreItemEntity Build()
    {
        return _entity;
    }

    public static StoreItemEntityBuilder GivenAStoreItem()
    {
        return new();
    }

    public StoreItemEntityBuilder WithId(Guid id)
    {
        _entity = _entity with {Id = id};
        return this;
    }

    public StoreItemEntityBuilder WithCode(string code)
    {
        _entity = _entity with {Code = code};
        return this;
    }

    public StoreItemEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
