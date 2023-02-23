using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransactionBuilder : IBuilder<Transaction>, ICollectionBuilder<Transaction>
{
    private readonly Faker<Transaction> _faker = new AutoFaker<Transaction>()
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.CashflowId, _ => null)
        .RuleFor(x => x.CashflowDate, _ => null)
        .RuleFor(x => x.Cashflow, _ => null)
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public Transaction Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public Transaction[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }

    public Transaction[] Build(Faker faker) => Build(faker.Random.Int(1, 10));

    public static TransactionBuilder GivenATransaction() => new();

    public TransactionBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(f => f.Amount, amount);
        return this;
    }

    public TransactionBuilder OnDate(DateTime date)
    {
        _faker.RuleFor(f => f.Date, date);
        return this;
    }

    public TransactionBuilder ForCashflowDate(DateTime date)
    {
        _faker.RuleFor(f => f.CashflowDate, date);
        return this;
    }

    public TransactionBuilder ForAccount(Account entity)
    {
        _faker
            .RuleFor(f => f.AccountId, entity.Id)
            .RuleFor(f => f.Account, entity)
            .RuleFor(f => f.UserId, entity.UserId);
        return this;
    }

    public TransactionBuilder ForCategory(Category entity)
    {
        _faker
            .RuleFor(f => f.CategoryId, entity.Id)
            .RuleFor(f => f.Category, entity);
        return this;
    }
}
