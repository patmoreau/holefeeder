using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Builders;

internal class AccountEntityBuilder : IBuilder<AccountEntity>, ICollectionBuilder<AccountEntity>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    private readonly Faker<AccountEntity> _faker = new AutoFaker<AccountEntity>()
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX))
        .RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Description, faker => faker.Random.Words());

    public static AccountEntityBuilder GivenAnActiveAccount()
    {
        var builder = new AccountEntityBuilder();
        builder._faker.RuleFor(f => f.Inactive, false);
        return builder;
    }

    public static AccountEntityBuilder GivenAnInactiveAccount()
    {
        var builder = new AccountEntityBuilder();
        builder._faker.RuleFor(f => f.Inactive, true);
        return builder;
    }

    public static AccountEntityBuilder GivenAnExistingAccount(AccountEntity entity)
    {
        var builder = new AccountEntityBuilder();
        builder._faker
            .RuleFor(f => f.Id, entity.Id)
            .RuleFor(f => f.Type, entity.Type)
            .RuleFor(f => f.Name, entity.Name)
            .RuleFor(f => f.Favorite, entity.Favorite)
            .RuleFor(f => f.OpenBalance, entity.OpenBalance)
            .RuleFor(f => f.OpenDate, entity.OpenDate)
            .RuleFor(f => f.Description, entity.Description)
            .RuleFor(f => f.Inactive, entity.Inactive)
            .RuleFor(f => f.UserId, entity.UserId);
        return builder;
    }


    public AccountEntityBuilder WithId(Guid id)
    {
        _faker.RuleFor(f => f.Id, id);
        return this;
    }

    public AccountEntityBuilder OfType(AccountType type)
    {
        _faker.RuleFor(f => f.Type, type);
        return this;
    }

    public AccountEntityBuilder IsFavorite(bool favorite)
    {
        _faker.RuleFor(f => f.Favorite, favorite);
        return this;
    }

    public AccountEntityBuilder WithName(string name)
    {
        _faker.RuleFor(f => f.Name, name);
        return this;
    }

    public AccountEntityBuilder WithDescription(string description)
    {
        _faker.RuleFor(f => f.Description, description);
        return this;
    }

    public AccountEntityBuilder WithOpenBalance(decimal openBalance)
    {
        _faker.RuleFor(f => f.OpenBalance, openBalance);
        return this;
    }

    public AccountEntityBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public AccountEntity Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public AccountEntity[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
