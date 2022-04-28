using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class TransactionEntityBuilder : IEntityBuilder<TransactionEntity>
{
    private TransactionEntity _entity;

    public static TransactionEntityBuilder GivenATransaction() => new();

    private TransactionEntityBuilder() => _entity = new TransactionEntityFactory().Generate();

    public TransactionEntityBuilder ForAccount(AccountEntity entity)
    {
        _entity = _entity with {AccountId = entity.Id, UserId = entity.UserId};
        return this;
    }

    public TransactionEntityBuilder ForCategory(CategoryEntity entity)
    {
        _entity = _entity with {CategoryId = entity.Id};
        return this;
    }

    public TransactionEntity Build() => _entity;
}
