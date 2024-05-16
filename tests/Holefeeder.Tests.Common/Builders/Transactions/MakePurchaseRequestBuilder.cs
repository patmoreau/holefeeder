using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class MakePurchaseRequestBuilder : FakerBuilder<Request>
{
    public override Request Build()
    {
        var a = base.Build();
        return a;
    }

    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.CategoryId, faker => faker.Random.Guid())
        .RuleFor(x => x.Cashflow, () => null)
        // .RuleForType(typeof(Request.CashflowRequest), _ => CashflowRequestBuilder.GivenACashflowPurchase().Build())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public MakePurchaseRequestBuilder ForAccount(Account account)
    {
        FakerRules.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public MakePurchaseRequestBuilder ForCategory(Category category)
    {
        FakerRules.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public MakePurchaseRequestBuilder OfAmount(decimal amount)
    {
        FakerRules.RuleFor(x => x.Amount, amount);
        return this;
    }

    public static MakePurchaseRequestBuilder GivenAPurchase() => new();
}

internal class CashflowRequestBuilder : FakerBuilder<Request.CashflowRequest>
{
    protected override Faker<Request.CashflowRequest> FakerRules { get; } = new Faker<Request.CashflowRequest>()
        .CustomInstantiator(faker => new Request.CashflowRequest(faker.Date.RecentDateOnly(),
            faker.PickRandom<DateIntervalType>(DateIntervalType.List), faker.Random.Int(1), faker.Random.Int(0)))
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0));

    public static CashflowRequestBuilder GivenACashflowPurchase() => new();
}
