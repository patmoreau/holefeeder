using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyCashflowRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>()
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount(1, Constants.MAX_AMOUNT))
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public ModifyCashflowRequestBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyCashflowRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyCashflowRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static ModifyCashflowRequestBuilder GivenAModifyCashflowRequest() => new();

    public static ModifyCashflowRequestBuilder GivenAnInvalidModifyCashflowRequest() =>
        new ModifyCashflowRequestBuilder().WithNoId();
}
