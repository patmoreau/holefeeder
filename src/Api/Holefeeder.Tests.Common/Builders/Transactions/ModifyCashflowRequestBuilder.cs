using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyCashflowRequestBuilder : RootBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new AutoFaker<Request>()
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
