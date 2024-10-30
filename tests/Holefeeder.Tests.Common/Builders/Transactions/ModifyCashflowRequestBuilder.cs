using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateFaker<Request>()
        .RuleFor(x => x.Id, faker => (CashflowId)faker.RandomGuid())
        .RuleFor(x => x.Amount, faker => MoneyBuilder.Create().Build())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public ModifyCashflowRequestBuilder OfAmount(Money amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyCashflowRequestBuilder WithId(CashflowId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyCashflowRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, CashflowId.Empty);
        return this;
    }

    public static ModifyCashflowRequestBuilder GivenAModifyCashflowRequest() => new();

    public static ModifyCashflowRequestBuilder GivenAnInvalidModifyCashflowRequest() =>
        new ModifyCashflowRequestBuilder().WithNoId();
}
