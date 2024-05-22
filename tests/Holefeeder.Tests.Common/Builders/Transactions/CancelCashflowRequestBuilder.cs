using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CancelCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.RandomGuid()));

    public CancelCashflowRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public CancelCashflowRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CancelCashflowRequestBuilder GivenACancelCashflowRequest() => new();

    public static CancelCashflowRequestBuilder GivenAnInvalidCancelCashflowRequest() =>
        new CancelCashflowRequestBuilder().WithNoId();
}
