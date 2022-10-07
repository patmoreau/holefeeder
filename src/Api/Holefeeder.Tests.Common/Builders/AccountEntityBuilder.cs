using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class AccountEntityBuilder : IBuilder<AccountEntity>
{
    private AccountEntity _entity;

    private AccountEntityBuilder(bool inactive)
    {
        _entity = new AccountEntityFactory(inactive).Generate();
    }

    private AccountEntityBuilder(AccountEntity entity)
    {
        _entity = entity;
    }

    public AccountEntity Build()
    {
        return _entity;
    }

    public static AccountEntityBuilder GivenAnActiveAccount()
    {
        return new(false);
    }

    public static AccountEntityBuilder GivenAnInactiveAccount()
    {
        return new(true);
    }

    public static AccountEntityBuilder GivenAnExistingAccount(AccountEntity entity)
    {
        return new(entity);
    }

    public AccountEntityBuilder WithId(Guid id)
    {
        _entity = _entity with {Id = id};
        return this;
    }

    public AccountEntityBuilder OfType(AccountType type)
    {
        _entity = _entity with {Type = type};
        return this;
    }

    public AccountEntityBuilder IsFavorite(bool favorite)
    {
        _entity = _entity with {Favorite = favorite};
        return this;
    }

    public AccountEntityBuilder WithName(string name)
    {
        _entity = _entity with {Name = name};
        return this;
    }

    public AccountEntityBuilder WithDescription(string description)
    {
        _entity = _entity with {Description = description};
        return this;
    }

    public AccountEntityBuilder WithOpenBalance(decimal openBalance)
    {
        _entity = _entity with {OpenBalance = openBalance};
        return this;
    }

    public AccountEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }
}
