using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CashflowBuilder : IBuilder<Cashflow>, ICollectionBuilder<Cashflow>
{
    private readonly Faker<Cashflow> _faker = new AutoFaker<Cashflow>()
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray())
        .RuleFor(x => x.Account, _ => null)
        .RuleFor(x => x.Category, _ => null)
        .RuleFor(x => x.Transactions, new List<Transaction>());

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

    public Cashflow[] Build(Faker faker) => Build(faker.Random.Int(1, 10));

    public static CashflowBuilder GivenAnActiveCashflow()
    {
        CashflowBuilder builder = new CashflowBuilder();
        builder._faker.RuleFor(x => x.Inactive, false);
        return builder;
    }

    public static CashflowBuilder GivenAnInactiveCashflow()
    {
        CashflowBuilder builder = new CashflowBuilder();
        builder._faker.RuleFor(x => x.Inactive, true);
        return builder;
    }

    public CashflowBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CashflowBuilder OnEffectiveDate(DateTime effectiveDate)
    {
        _faker.RuleFor(x => x.EffectiveDate, effectiveDate);
        return this;
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

    public CashflowBuilder ForAccount(Guid id)
    {
        _faker.RuleFor(x => x.AccountId, id);
        return this;
    }

    public CashflowBuilder ForCategory(Category entity)
    {
        _faker.RuleFor(x => x.CategoryId, entity.Id);
        _faker.RuleFor(x => x.Category, entity);
        return this;
    }

    public CashflowBuilder ForCategory(Guid id)
    {
        _faker.RuleFor(x => x.CategoryId, id);
        return this;
    }

    public CashflowBuilder ForUser(Guid userId)
    {
        _faker.RuleFor(x => x.UserId, userId);
        return this;
    }

    public CashflowBuilder OfFrequency(int frequency = 1)
    {
        _faker.RuleFor(x => x.Frequency, frequency);
        return this;
    }

    public CashflowBuilder OfFrequency(DateIntervalType intervalType, int frequency = 1)
    {
        _faker.RuleFor(x => x.IntervalType, intervalType);
        return OfFrequency(frequency);
    }

    public CashflowBuilder Recurring(int recurrence)
    {
        _faker.RuleFor(x => x.Recurrence, recurrence);
        return this;
    }

    public CashflowBuilder WithTransactions()
    {
        _faker.RuleFor(x => x.Transactions, GivenATransaction().Build);
        return this;
    }

    public CashflowBuilder WithTransactions(params Transaction[] transactions)
    {
        _faker.RuleFor(x => x.Transactions, transactions);
        return this;
    }
}
