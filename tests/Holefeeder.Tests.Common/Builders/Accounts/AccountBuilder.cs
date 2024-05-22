using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class AccountBuilder : FakerBuilder<Account>
{
    public override Account Build()
    {
        var a = base.Build();
        return a;
    }

    protected override Faker<Account> FakerRules { get; } = new Faker<Account>()
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
        .RuleFor(x => x.Favorite, faker => faker.Random.Bool())
        .RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount())
        .RuleFor(x => x.OpenDate, faker => faker.Date.PastDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Inactive, faker => faker.Random.Bool())
        .RuleFor(x => x.UserId, faker => Guid.NewGuid())
        .RuleFor(x => x.Transactions, new List<Transaction>())
        .RuleFor(x => x.Cashflows, new List<Cashflow>());

    public static AccountBuilder GivenAnActiveAccount()
    {
        AccountBuilder builder = new();
        builder.FakerRules.RuleFor(f => f.Inactive, false);
        return builder;
    }

    public static AccountBuilder GivenAnInactiveAccount()
    {
        AccountBuilder builder = new();
        builder.FakerRules.RuleFor(f => f.Inactive, true);
        return builder;
    }

    public static AccountBuilder GivenAnExistingAccount(Account entity)
    {
        AccountBuilder builder = new();
        builder.FakerRules
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
        FakerRules.RuleFor(f => f.Id, id);
        return this;
    }

    public AccountBuilder WithActiveCashflows()
    {
        FakerRules.RuleFor(f => f.Cashflows, GivenAnActiveCashflow().BuildCollection());
        return this;
    }

    public AccountBuilder OfType(AccountType type)
    {
        FakerRules.RuleFor(f => f.Type, type);
        return this;
    }

    public AccountBuilder IsFavorite(bool favorite)
    {
        FakerRules.RuleFor(f => f.Favorite, favorite);
        return this;
    }

    public AccountBuilder WithName(string name)
    {
        FakerRules.RuleFor(f => f.Name, name);
        return this;
    }

    public AccountBuilder WithDescription(string description)
    {
        FakerRules.RuleFor(f => f.Description, description);
        return this;
    }

    public AccountBuilder WithOpenBalance(decimal openBalance)
    {
        FakerRules.RuleFor(f => f.OpenBalance, openBalance);
        return this;
    }

    public AccountBuilder WithOpenDate(DateOnly openDate)
    {
        FakerRules.RuleFor(f => f.OpenDate, openDate);
        return this;
    }

    public AccountBuilder ForUser(Guid userId)
    {
        FakerRules.RuleFor(f => f.UserId, userId);
        return this;
    }

    public AccountBuilder ForNoUser()
    {
        FakerRules.RuleFor(f => f.UserId, Guid.Empty);
        return this;
    }
}
