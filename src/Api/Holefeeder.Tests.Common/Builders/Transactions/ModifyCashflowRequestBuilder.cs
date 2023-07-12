using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public ModifyCashflowRequestBuilder OfAmount(decimal amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyCashflowRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyCashflowRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static ModifyCashflowRequestBuilder GivenAModifyCashflowRequest() => new();

    public static ModifyCashflowRequestBuilder GivenAnInvalidModifyCashflowRequest() =>
        new ModifyCashflowRequestBuilder().WithNoId();
}
