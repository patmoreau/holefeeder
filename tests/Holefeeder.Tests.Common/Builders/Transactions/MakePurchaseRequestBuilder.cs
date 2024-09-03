using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class MakePurchaseRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateFaker()
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, faker => MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.CategoryId, faker => (CategoryId)faker.RandomGuid())
        .RuleFor(x => x.Cashflow, _ => null)
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public MakePurchaseRequestBuilder ForAccount(Account account)
    {
        Faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public MakePurchaseRequestBuilder ForNoAccount()
    {
        Faker.RuleFor(x => x.AccountId, AccountId.Empty);
        return this;
    }

    public MakePurchaseRequestBuilder ForCategory(Category category)
    {
        Faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public MakePurchaseRequestBuilder OfAmount(Money amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public MakePurchaseRequestBuilder WithNoDate()
    {
        Faker.RuleFor(x => x.Date, DateOnly.MinValue);
        return this;
    }

    public static MakePurchaseRequestBuilder GivenAPurchase() => new();
}

internal class CashflowRequestBuilder : FakerBuilder<Request.CashflowRequest>
{
    protected override Faker<Request.CashflowRequest> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0));

    public static CashflowRequestBuilder GivenACashflowPurchase() => new();
}
