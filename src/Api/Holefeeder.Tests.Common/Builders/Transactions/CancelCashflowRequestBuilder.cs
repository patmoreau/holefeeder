using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CancelCashflowRequestBuilder : RootBuilder<Request>
{
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
