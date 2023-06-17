using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CashflowBuilder : FakerBuilder<Cashflow>
{
    protected override Faker<Cashflow> Faker { get; } = new Faker<Cashflow>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.Past().Date)
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0))
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(min: 1))
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray())
        .RuleFor(x => x.AccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.Account, _ => null)
        .RuleFor(x => x.CategoryId, faker => faker.Random.Guid())
        .RuleFor(x => x.Category, _ => null)
        .RuleFor(x => x.Transactions, new List<Transaction>())
        .RuleFor(x => x.UserId, faker => faker.Random.Guid());

    public static CashflowBuilder GivenAnActiveCashflow()
    {
        CashflowBuilder builder = new();
        builder.Faker.RuleFor(x => x.Inactive, false);
        return builder;
    }

    public static CashflowBuilder GivenAnInactiveCashflow()
    {
        CashflowBuilder builder = new();
        builder.Faker.RuleFor(x => x.Inactive, true);
        return builder;
    }

    public CashflowBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CashflowBuilder OnEffectiveDate(DateTime effectiveDate)
    {
        Faker.RuleFor(x => x.EffectiveDate, effectiveDate);
        return this;
    }

    public CashflowBuilder OfAmount(decimal amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public CashflowBuilder ForAccount(Account entity)
    {
        Faker.RuleFor(x => x.AccountId, entity.Id);
        Faker.RuleFor(x => x.Account, entity);
        return this;
    }

    public CashflowBuilder ForAccount(Guid id)
    {
        Faker.RuleFor(x => x.AccountId, id);
        return this;
    }

    public CashflowBuilder ForCategory(Category entity)
    {
        Faker.RuleFor(x => x.CategoryId, entity.Id);
        Faker.RuleFor(x => x.Category, entity);
        return this;
    }

    public CashflowBuilder ForCategory(Guid id)
    {
        Faker.RuleFor(x => x.CategoryId, id);
        return this;
    }

    public CashflowBuilder ForUser(Guid userId)
    {
        Faker.RuleFor(x => x.UserId, userId);
        return this;
    }

    public CashflowBuilder OfFrequency(int frequency = 1)
    {
        Faker.RuleFor(x => x.Frequency, frequency);
        return this;
    }

    public CashflowBuilder OfFrequency(DateIntervalType intervalType, int frequency = 1)
    {
        Faker.RuleFor(x => x.IntervalType, intervalType);
        return OfFrequency(frequency);
    }

    public CashflowBuilder Recurring(int recurrence)
    {
        Faker.RuleFor(x => x.Recurrence, recurrence);
        return this;
    }

    public CashflowBuilder WithTransactions()
    {
        Faker.RuleFor(x => x.Transactions, GivenATransaction().BuildCollection());
        return this;
    }

    public CashflowBuilder WithTransactions(params Transaction[] transactions)
    {
        Faker.RuleFor(x => x.Transactions, transactions);
        return this;
    }
}
