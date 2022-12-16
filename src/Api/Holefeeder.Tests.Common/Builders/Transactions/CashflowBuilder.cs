using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CashflowBuilder : IBuilder<Cashflow>, ICollectionBuilder<Cashflow>
{
    private const decimal AMOUNT_MAX = 100m;

    private readonly Faker<Cashflow> _faker = new AutoFaker<Cashflow>()
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: decimal.Zero, max: AMOUNT_MAX))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(min: 1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(min: 0))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public Cashflow Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public Cashflow[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }

    public static CashflowBuilder GivenAnActiveCashflow()
    {
        var builder = new CashflowBuilder();
        builder._faker.RuleFor(x => x.Inactive, false);
        return builder;
    }

    public CashflowBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public CashflowBuilder ForAccount(Account entity)
    {
        _faker.RuleFor(x => x.AccountId, entity.Id);
        _faker.RuleFor(x => x.Account, entity);
        return this;
    }

    public CashflowBuilder ForCategory(Category entity)
    {
        _faker.RuleFor(x => x.CategoryId, entity.Id);
        _faker.RuleFor(x => x.Category, entity);
        return this;
    }

    public CashflowBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(x => x.UserId, userId);
        return this;
    }
}
