using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class AccountBuilder : IBuilder<Account>, ICollectionBuilder<Account>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    private readonly Faker<Account> _faker = new AutoFaker<Account>()
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX))
        .RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Description, faker => faker.Random.Words())
        .RuleFor(x => x.Transactions, new List<Transaction>());

    public static AccountBuilder GivenAnActiveAccount()
    {
        var builder = new AccountBuilder();
        builder._faker.RuleFor(f => f.Inactive, false);
        return builder;
    }

    public static AccountBuilder GivenAnInactiveAccount()
    {
        var builder = new AccountBuilder();
        builder._faker.RuleFor(f => f.Inactive, true);
        return builder;
    }

    public static AccountBuilder GivenAnExistingAccount(Account entity)
    {
        var builder = new AccountBuilder();
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

    public AccountBuilder WithId(Guid id)
    {
        _faker.RuleFor(f => f.Id, id);
        return this;
    }

    public AccountBuilder WithNoCashflows()
    {
        _faker.RuleFor(f => f.Cashflows, Array.Empty<Guid>());
        return this;
    }

    public AccountBuilder OfType(AccountType type)
    {
        _faker.RuleFor(f => f.Type, type);
        return this;
    }

    public AccountBuilder IsFavorite(bool favorite)
    {
        _faker.RuleFor(f => f.Favorite, favorite);
        return this;
    }

    public AccountBuilder WithName(string name)
    {
        _faker.RuleFor(f => f.Name, name);
        return this;
    }

    public AccountBuilder WithDescription(string description)
    {
        _faker.RuleFor(f => f.Description, description);
        return this;
    }

    public AccountBuilder WithOpenBalance(decimal openBalance)
    {
        _faker.RuleFor(f => f.OpenBalance, openBalance);
        return this;
    }

    public AccountBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public Account Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public Account[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
