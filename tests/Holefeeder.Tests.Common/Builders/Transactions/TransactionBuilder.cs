using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransactionBuilder : FakerBuilder<Transaction>
{
    protected override Faker<Transaction> Faker { get; } = CreatePrivateFaker<Transaction>()
        .RuleFor(x => x.Id, faker => (TransactionId)faker.RandomGuid())
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.Account, _ => default)
        .RuleFor(x => x.CategoryId, faker => (CategoryId)faker.RandomGuid())
        .RuleFor(x => x.Category, _ => default)
        .RuleFor(x => x.CashflowId, _ => default)
        .RuleFor(x => x.CashflowDate, _ => default)
        .RuleFor(x => x.Cashflow, _ => default)
        .RuleFor(x => x.UserId, faker => (UserId)faker.RandomGuid())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public static TransactionBuilder GivenATransaction() => new();

    public TransactionBuilder WithId(TransactionId id)
    {
        Faker.RuleFor(f => f.Id, id);
        return this;
    }

    public TransactionBuilder WithNoId()
    {
        Faker.RuleFor(f => f.Id, _ => TransactionId.Empty);
        return this;
    }

    public TransactionBuilder OfAmount(Money amount)
    {
        Faker.RuleFor(f => f.Amount, amount);
        return this;
    }

    public TransactionBuilder OnDate(DateOnly date)
    {
        Faker.RuleFor(f => f.Date, date);
        return this;
    }

    public TransactionBuilder WithNoDate()
    {
        Faker.RuleFor(f => f.Date, _ => default);
        return this;
    }

    public TransactionBuilder ForCashflowDate(DateOnly date)
    {
        Faker.RuleFor(f => f.CashflowDate, date);
        return this;
    }

    public TransactionBuilder ForAccount(Account entity, bool includeAccount = false)
    {
        Faker.RuleFor(f => f.AccountId, entity.Id)
            .RuleFor(f => f.UserId, entity.UserId);
        if (includeAccount)
        {
            Faker.RuleFor(f => f.Account, entity);
        }
        return this;
    }

    public TransactionBuilder WithNoAccount()
    {
        Faker
            .RuleFor(f => f.AccountId, _ => AccountId.Empty)
            .RuleFor(f => f.Account, _ => default);
        return this;
    }

    public TransactionBuilder ForCategory(Category entity, bool includeCategory = false)
    {
        Faker.RuleFor(f => f.CategoryId, entity.Id);
        if (includeCategory)
        {
            Faker.RuleFor(f => f.Category, entity);
        }
        return this;
    }

    public TransactionBuilder WithNoCategory()
    {
        Faker
            .RuleFor(f => f.CategoryId, _ => CategoryId.Empty)
            .RuleFor(f => f.Category, _ => default);
        return this;
    }

    public TransactionBuilder ForUser(UserId userId)
    {
        Faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public TransactionBuilder WithNoUser()
    {
        Faker.RuleFor(f => f.UserId, _ => UserId.Empty);
        return this;
    }

    public TransactionBuilder WithTags(IEnumerable<string> tags)
    {
        Faker.RuleFor(f => f.Tags, tags);
        return this;
    }

    public TransactionBuilder WithNoTags()
    {
        Faker.RuleFor(f => f.Tags, _ => Array.Empty<string>());
        return this;
    }
}
