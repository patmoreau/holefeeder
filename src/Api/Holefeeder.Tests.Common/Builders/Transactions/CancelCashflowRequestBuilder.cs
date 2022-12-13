using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class CancelCashflowRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public CancelCashflowRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public CancelCashflowRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static CancelCashflowRequestBuilder GivenACancelCashflowRequest() => new();

    public static CancelCashflowRequestBuilder GivenAnInvalidCancelCashflowRequest() =>
        new CancelCashflowRequestBuilder().WithNoId();
}
