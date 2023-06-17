using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransactionBuilder : FakerBuilder<Transaction>
{
    protected override Faker<Transaction> Faker { get; } = new Faker<Transaction>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Date, faker => faker.Date.Recent())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: 1))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.Account, _ => default)
        .RuleFor(x => x.CategoryId, faker => faker.Random.Guid())
        .RuleFor(x => x.Category, _ => default)
        .RuleFor(x => x.CashflowId, _ => default)
        .RuleFor(x => x.CashflowDate, _ => default)
        .RuleFor(x => x.Cashflow, _ => default)
        .RuleFor(x => x.UserId, faker => faker.Random.Guid())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public static TransactionBuilder GivenATransaction() => new();

    public TransactionBuilder WithNoId()
    {
        Faker.RuleFor(f => f.Id, _ => default);
        return this;
    }

    public TransactionBuilder OfAmount(decimal amount)
    {
        Faker.RuleFor(f => f.Amount, amount);
        return this;
    }

    public TransactionBuilder WithNegativeAmount()
    {
        Faker.RuleFor(f => f.Amount, faker => faker.Finance.Amount(decimal.MinValue, decimal.MinusOne));
        return this;
    }

    public TransactionBuilder OnDate(DateTime date)
    {
        Faker.RuleFor(f => f.Date, date);
        return this;
    }

    public TransactionBuilder WithNoDate()
    {
        Faker.RuleFor(f => f.Date, _ => default);
        return this;
    }

    public TransactionBuilder ForCashflowDate(DateTime date)
    {
        Faker.RuleFor(f => f.CashflowDate, date);
        return this;
    }

    public TransactionBuilder ForAccount(Account entity)
    {
        Faker
            .RuleFor(f => f.AccountId, entity.Id)
            .RuleFor(f => f.Account, entity)
            .RuleFor(f => f.UserId, entity.UserId);
        return this;
    }

    public TransactionBuilder WithNoAccount()
    {
        Faker
            .RuleFor(f => f.AccountId, _ => default)
            .RuleFor(f => f.Account, _ => default);
        return this;
    }

    public TransactionBuilder ForCategory(Category entity)
    {
        Faker
            .RuleFor(f => f.CategoryId, entity.Id)
            .RuleFor(f => f.Category, entity);
        return this;
    }

    public TransactionBuilder WithNoCategory()
    {
        Faker
            .RuleFor(f => f.CategoryId, _ => default)
            .RuleFor(f => f.Category, _ => default);
        return this;
    }

    public TransactionBuilder WithNoUser()
    {
        Faker.RuleFor(f => f.UserId, _ => default);
        return this;
    }
}
