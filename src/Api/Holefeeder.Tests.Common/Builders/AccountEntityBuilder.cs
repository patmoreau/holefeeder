using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.Tests.Common.Builders;

internal class AccountEntityBuilder : IEntityBuilder<AccountEntity>
{
    private AccountEntity _entity;

    public static AccountEntityBuilder GivenAnActiveAccount() => new(false);

    public static AccountEntityBuilder GivenAnInactiveAccount() => new(true);

    private AccountEntityBuilder(bool inactive) => _entity = new AccountEntityFactory(inactive).Generate();

    public AccountEntityBuilder OfType(AccountType type)
    {
        _entity = _entity with {Type = type};
        return this;
    }

    public AccountEntityBuilder WithName(string name)
    {
        _entity = _entity with {Name = name};
        return this;
    }

    public AccountEntityBuilder ForUser(Guid userId)
    {
        _entity = _entity with {UserId = userId};
        return this;
    }

    public AccountEntity Build() => _entity;
}
