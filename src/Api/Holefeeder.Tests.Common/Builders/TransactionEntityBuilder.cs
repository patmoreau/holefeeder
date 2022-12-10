using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Builders;

internal class TransactionEntityBuilder : IBuilder<TransactionEntity>, ICollectionBuilder<TransactionEntity>
{
    private const decimal AMOUNT_MAX = 100m;

    private readonly Faker<TransactionEntity> _faker = new AutoFaker<TransactionEntity>()
        .RuleFor(x => x.Date, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: Decimal.Zero, max: AMOUNT_MAX))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.CashflowId, _ => null)
        .RuleFor(x => x.CashflowDate, _ => null)
        .RuleFor(x => x.Tags, faker => string.Join(',', faker.Random.WordsArray(0, 5)));

    public static TransactionEntityBuilder GivenATransaction() => new();

    public TransactionEntityBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(f => f.Amount, amount);
        return this;
    }

    public TransactionEntityBuilder ForAccount(AccountEntity entity)
    {
        _faker
            .RuleFor(f => f.AccountId, entity.Id)
            .RuleFor(f => f.UserId, entity.UserId);
        return this;
    }

    public TransactionEntityBuilder ForCategory(Category entity)
    {
        _faker.RuleFor(f => f.CategoryId, entity.Id);
        return this;
    }

    public TransactionEntity Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public TransactionEntity[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
