using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CashflowBuilder : FakerBuilder<Cashflow>
{
    public override Cashflow Build()
    {
        var a = base.Build();
        return a;
    }

    protected override Faker<Cashflow> FakerRules { get; } = new Faker<Cashflow>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.PastDateOnly())
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
        builder.FakerRules.RuleFor(x => x.Inactive, false);
        return builder;
    }

    public static CashflowBuilder GivenAnInactiveCashflow()
    {
        CashflowBuilder builder = new();
        builder.FakerRules.RuleFor(x => x.Inactive, true);
        return builder;
    }

    public CashflowBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public CashflowBuilder OnEffectiveDate(DateOnly effectiveDate)
    {
        FakerRules.RuleFor(x => x.EffectiveDate, effectiveDate);
        return this;
    }

    public CashflowBuilder OfAmount(decimal amount)
    {
        FakerRules.RuleFor(x => x.Amount, amount);
        return this;
    }

    public CashflowBuilder ForAccount(Account entity)
    {
        FakerRules.RuleFor(x => x.AccountId, entity.Id);
        return this;
    }

    public CashflowBuilder ForAccount(Guid id)
    {
        FakerRules.RuleFor(x => x.AccountId, id);
        return this;
    }

    public CashflowBuilder ForCategory(Category entity)
    {
        FakerRules.RuleFor(x => x.CategoryId, entity.Id);
        return this;
    }

    public CashflowBuilder ForCategory(Guid id)
    {
        FakerRules.RuleFor(x => x.CategoryId, id);
        return this;
    }

    public CashflowBuilder ForUser(Guid userId)
    {
        FakerRules.RuleFor(x => x.UserId, userId);
        return this;
    }

    public CashflowBuilder OfFrequency(int frequency = 1)
    {
        FakerRules.RuleFor(x => x.Frequency, frequency);
        return this;
    }

    public CashflowBuilder OfFrequency(DateIntervalType intervalType, int frequency = 1)
    {
        FakerRules.RuleFor(x => x.IntervalType, intervalType);
        return OfFrequency(frequency);
    }

    public CashflowBuilder Recurring(int recurrence)
    {
        FakerRules.RuleFor(x => x.Recurrence, recurrence);
        return this;
    }

    public CashflowBuilder WithTransactions()
    {
        FakerRules.RuleFor(x => x.Transactions, GivenATransaction().BuildCollection());
        return this;
    }

    public CashflowBuilder WithTransactions(params Transaction[] transactions)
    {
        FakerRules.RuleFor(x => x.Transactions, transactions);
        return this;
    }
}
