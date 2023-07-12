using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Transactions;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class AccountBuilder : FakerBuilder<Account>
{
    protected override Faker<Account> Faker { get; } = new Faker<Account>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.Favorite, faker => faker.Random.Bool())
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount())
        .RuleFor(x => x.OpenDate, faker => faker.Date.PastDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Inactive, faker => faker.Random.Bool())
        .RuleFor(x => x.UserId, faker => faker.Random.Guid())
        .RuleFor(x => x.Transactions, new List<Transaction>())
        .RuleFor(x => x.Cashflows, new List<Cashflow>());

    public static AccountBuilder GivenAnActiveAccount()
    {
        AccountBuilder builder = new();
        builder.Faker.RuleFor(f => f.Inactive, false);
        return builder;
    }

    public static AccountBuilder GivenAnInactiveAccount()
    {
        AccountBuilder builder = new();
        builder.Faker.RuleFor(f => f.Inactive, true);
        return builder;
    }

    public static AccountBuilder GivenAnExistingAccount(Account entity)
    {
        AccountBuilder builder = new();
        builder.Faker
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
        Faker.RuleFor(f => f.Id, id);
        return this;
    }

    public AccountBuilder WithActiveCashflows()
    {
        Faker.RuleFor(f => f.Cashflows, GivenAnActiveCashflow().BuildCollection());
        return this;
    }

    public AccountBuilder OfType(AccountType type)
    {
        Faker.RuleFor(f => f.Type, type);
        return this;
    }

    public AccountBuilder IsFavorite(bool favorite)
    {
        Faker.RuleFor(f => f.Favorite, favorite);
        return this;
    }

    public AccountBuilder WithName(string name)
    {
        Faker.RuleFor(f => f.Name, name);
        return this;
    }

    public AccountBuilder WithDescription(string description)
    {
        Faker.RuleFor(f => f.Description, description);
        return this;
    }

    public AccountBuilder WithOpenBalance(decimal openBalance)
    {
        Faker.RuleFor(f => f.OpenBalance, openBalance);
        return this;
    }

    public AccountBuilder WithOpenDate(DateOnly openDate)
    {
        Faker.RuleFor(f => f.OpenDate, openDate);
        return this;
    }

    public AccountBuilder ForUser(Guid userId)
    {
        Faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public AccountBuilder ForNoUser()
    {
        Faker.RuleFor(f => f.UserId, Guid.Empty);
        return this;
    }
}
