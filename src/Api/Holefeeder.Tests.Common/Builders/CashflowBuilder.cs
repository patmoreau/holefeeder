using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class CashflowBuilder : IBuilder<Cashflow>
{
    private Cashflow _entity;

    public static CashflowBuilder GivenACashflow() => new();

    private CashflowBuilder() => _entity = new CashflowFactory().Generate();

    public CashflowBuilder OfAmount(decimal amount)
    {
        _entity = _entity with {Amount = amount};
        return this;
    }

    public CashflowBuilder ForAccount(AccountEntity entity)
    {
        _entity = _entity with {AccountId = entity.Id};
        return this;
    }

    public CashflowBuilder ForCategory(CategoryEntity entity)
    {
        _entity = _entity with {CategoryId = entity.Id};
        return this;
    }

    public CashflowBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }

    public Cashflow Build() => _entity;
}
