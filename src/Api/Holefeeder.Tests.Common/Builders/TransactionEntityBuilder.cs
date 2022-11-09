using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class TransactionEntityBuilder : IBuilder<TransactionEntity>
{
    private TransactionEntity _entity;

    private TransactionEntityBuilder()
    {
        _entity = new TransactionEntityFactory().Generate();
    }

    public TransactionEntity Build()
    {
        return _entity;
    }

    public static TransactionEntityBuilder GivenATransaction()
    {
        return new();
    }

    public TransactionEntityBuilder OfAmount(decimal amount)
    {
        _entity = _entity with {Amount = amount};
        return this;
    }

    public TransactionEntityBuilder ForAccount(AccountEntity entity)
    {
        _entity = _entity with {AccountId = entity.Id, UserId = entity.UserId};
        return this;
    }

    public TransactionEntityBuilder ForCategory(Category entity)
    {
        _entity = _entity with {CategoryId = entity.Id};
        return this;
    }
}
