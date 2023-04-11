using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class MakePurchaseRequestBuilder : RootBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new AutoFaker<Request>()
        .RuleForType(typeof(Request.CashflowRequest), _ => CashflowRequestBuilder.GivenACashflowPurchase().Build())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public MakePurchaseRequestBuilder ForAccount(Account account)
    {
        Faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public MakePurchaseRequestBuilder ForCategory(Category category)
    {
        Faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public MakePurchaseRequestBuilder OfAmount(decimal amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public static MakePurchaseRequestBuilder GivenAPurchase() => new();
}

internal class CashflowRequestBuilder : RootBuilder<Request.CashflowRequest>
{
    protected override Faker<Request.CashflowRequest> Faker { get; } = new AutoFaker<Request.CashflowRequest>()
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(0));

    public static CashflowRequestBuilder GivenACashflowPurchase() => new();
}
