using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class StoreItemEntityBuilder : IEntityBuilder<StoreItemEntity>
{
    private StoreItemEntity _entity;

    public static StoreItemEntityBuilder GivenAStoreItem() => new();

    private StoreItemEntityBuilder() => _entity = new StoreItemEntityFactory().Generate();

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

    public StoreItemEntity Build() => _entity;
}
