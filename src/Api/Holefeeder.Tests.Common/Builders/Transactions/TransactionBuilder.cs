using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class TransactionBuilder : IBuilder<Transaction>, ICollectionBuilder<Transaction>
{
    private const decimal AMOUNT_MAX = 100m;

    private readonly Faker<Transaction> _faker = new AutoFaker<Transaction>()
        .RuleFor(x => x.Date, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.CashflowId, _ => null)
        .RuleFor(x => x.CashflowDate, _ => null)
        .RuleFor(x => x.Tags, faker => faker.Random.WordsArray(0, 5));

    public static TransactionBuilder GivenATransaction() => new();

    public TransactionBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(f => f.Amount, amount);
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
}
