using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class MakePurchaseRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>()
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(1, Constants.MAX_AMOUNT))
        .RuleForType(typeof(Request.CashflowRequest), _ => CashflowRequestBuilder.GivenACashflowPurchase().Build())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public MakePurchaseRequestBuilder ForAccount(Account account)
    {
        _faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public MakePurchaseRequestBuilder ForCategory(Category category)
    {
        _faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public MakePurchaseRequestBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public static MakePurchaseRequestBuilder GivenAPurchase() => new();
}

internal class CashflowRequestBuilder : IBuilder<Request.CashflowRequest>
{
    private readonly Faker<Request.CashflowRequest> _faker = new AutoFaker<Request.CashflowRequest>()
        .RuleFor(x => x.Frequency, faker => faker.Random.Int(min: 1))
        .RuleFor(x => x.Recurrence, faker => faker.Random.Int(min: 0));

    public Request.CashflowRequest Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public static CashflowRequestBuilder GivenACashflowPurchase() => new();
}
