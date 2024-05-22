using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public ModifyCashflowRequestBuilder OfAmount(decimal amount)
    {
        FakerRules.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyCashflowRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyCashflowRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static ModifyCashflowRequestBuilder GivenAModifyCashflowRequest() => new();

    public static ModifyCashflowRequestBuilder GivenAnInvalidModifyCashflowRequest() =>
        new ModifyCashflowRequestBuilder().WithNoId();
}
