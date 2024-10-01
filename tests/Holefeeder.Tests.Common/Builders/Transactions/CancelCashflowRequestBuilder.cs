using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Transactions;

using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CancelCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.Id, faker => (CashflowId)faker.Random.Guid());

    public CancelCashflowRequestBuilder WithId(CashflowId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CancelCashflowRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, CashflowId.Empty);
        return this;
    }

    public static CancelCashflowRequestBuilder GivenACancelCashflowRequest() => new();

    public static CancelCashflowRequestBuilder GivenAnInvalidCancelCashflowRequest() =>
        new CancelCashflowRequestBuilder().WithNoId();
}
