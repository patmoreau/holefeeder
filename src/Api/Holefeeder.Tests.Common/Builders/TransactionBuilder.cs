using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class TransactionBuilder : IBuilder<Transaction>
{
    private Transaction _entity;

    private TransactionBuilder()
    {
        _entity = new TransactionFactory().Generate();
    }

    public Transaction Build()
    {
        return _entity;
    }

    public static TransactionBuilder ATransaction()
    {
        return new();
    }

    public TransactionBuilder OfAmount(decimal amount)
    {
        _entity = _entity with {Amount = amount};
        return this;
    }

    public TransactionBuilder ForAccount(AccountEntity entity)
    {
        _entity = _entity with {AccountId = entity.Id};
        return this;
    }

    public TransactionBuilder ForCategory(CategoryEntity entity)
    {
        _entity = _entity with {CategoryId = entity.Id};
        return this;
    }

    public TransactionBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
