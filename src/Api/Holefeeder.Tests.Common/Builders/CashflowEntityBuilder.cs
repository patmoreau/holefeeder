using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class CashflowEntityBuilder : IBuilder<CashflowEntity>
{
    private CashflowEntity _entity;

    private CashflowEntityBuilder()
    {
        _entity = new CashflowEntityFactory().Generate();
    }

    public CashflowEntity Build()
    {
        return _entity;
    }

    public static CashflowEntityBuilder GivenACashflowEntity()
    {
        return new();
    }

    public CashflowEntityBuilder OfAmount(decimal amount)
    {
        _entity = _entity with {Amount = amount};
        return this;
    }

    public CashflowEntityBuilder ForAccount(AccountEntity entity)
    {
        _entity = _entity with {AccountId = entity.Id, UserId = entity.UserId};
        return this;
    }

    public CashflowEntityBuilder ForCategory(Category entity)
    {
        _entity = _entity with {CategoryId = entity.Id};
        return this;
    }

    public CashflowEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
