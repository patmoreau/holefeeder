using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class AccountBuilder : FakerBuilder<Account>
{
    protected override Faker<Account> Faker { get; } = CreatePrivateFaker<Account>()
        .RuleFor(x => x.Id, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.Favorite, false)
        .RuleFor(x => x.OpenBalance, MoneyBuilder.Create().Build())
        .RuleFor(x => x.OpenDate, faker => faker.Date.PastDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Inactive, faker => faker.Random.Bool())
        .RuleFor(x => x.UserId, faker => (UserId)faker.Random.Guid())
        .RuleFor(x => x.Transactions, [])
        .RuleFor(x => x.Cashflows, []);

    public static AccountBuilder GivenAnActiveAccount()
    {
        var builder = new AccountBuilder();
        builder.Faker.RuleFor(f => f.Inactive, false);
        return builder;
    }

    public static AccountBuilder GivenAnInactiveAccount()
    {
        var builder = new AccountBuilder();
        builder.Faker.RuleFor(f => f.Inactive, true);
        return builder;
    }

    public static AccountBuilder GivenAnExistingAccount(Account entity)
    {
        var builder = new AccountBuilder();
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

    public AccountBuilder WithId(AccountId id)
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

    public AccountBuilder WithOpenBalance(Money openBalance)
    {
        Faker.RuleFor(f => f.OpenBalance, openBalance);
        return this;
    }

    public AccountBuilder WithOpenDate(DateOnly openDate)
    {
        Faker.RuleFor(f => f.OpenDate, openDate);
        return this;
    }

    public AccountBuilder ForUser(UserId userId)
    {
        Faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public AccountBuilder ForNoUser()
    {
        Faker.RuleFor(f => f.UserId, UserId.Empty);
        return this;
    }
}
