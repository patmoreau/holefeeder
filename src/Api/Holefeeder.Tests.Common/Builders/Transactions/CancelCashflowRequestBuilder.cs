using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CancelCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Guid()));

    public CancelCashflowRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CancelCashflowRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CancelCashflowRequestBuilder GivenACancelCashflowRequest() => new();

    public static CancelCashflowRequestBuilder GivenAnInvalidCancelCashflowRequest() =>
        new CancelCashflowRequestBuilder().WithNoId();
}
