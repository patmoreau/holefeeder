using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CashflowBuilder : FakerBuilder<Cashflow>
{
    protected override Faker<Cashflow> Faker { get; } = CreatePrivateFaker()
        .RuleFor(x => x.Id, faker => (CashflowId)faker.RandomGuid())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.PastDateOnly())
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0))
        .RuleFor(x => x.Amount, faker => MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray())
        .RuleFor(x => x.AccountId, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.Account, _ => null)
        .RuleFor(x => x.CategoryId, faker => (CategoryId)faker.RandomGuid())
        .RuleFor(x => x.Category, _ => null)
        .RuleFor(x => x.Transactions, new List<Transaction>())
        .RuleFor(x => x.UserId, faker => (UserId)faker.RandomGuid());

    public static CashflowBuilder GivenAnActiveCashflow()
    {
        var builder = new CashflowBuilder();
        builder.Faker.RuleFor(x => x.Inactive, false);
        return builder;
    }

    public static CashflowBuilder GivenAnInactiveCashflow()
    {
        var builder = new CashflowBuilder();
        builder.Faker.RuleFor(x => x.Inactive, true);
        return builder;
    }

    public CashflowBuilder WithId(CashflowId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CashflowBuilder OnEffectiveDate(DateOnly effectiveDate)
    {
        Faker.RuleFor(x => x.EffectiveDate, effectiveDate);
        return this;
    }

    public CashflowBuilder OfAmount(Money amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public CashflowBuilder ForAccount(Account entity)
    {
        Faker.RuleFor(x => x.AccountId, entity.Id);
        return this;
    }

    public CashflowBuilder ForAccount(AccountId id)
    {
        Faker.RuleFor(x => x.AccountId, id);
        return this;
    }

    public CashflowBuilder ForCategory(Category entity)
    {
        Faker.RuleFor(x => x.CategoryId, entity.Id);
        return this;
    }

    public CashflowBuilder ForCategory(CategoryId id)
    {
        Faker.RuleFor(x => x.CategoryId, id);
        return this;
    }

    public CashflowBuilder ForUser(UserId userId)
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
