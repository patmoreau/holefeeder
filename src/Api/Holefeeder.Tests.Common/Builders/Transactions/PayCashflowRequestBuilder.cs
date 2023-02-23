using Holefeeder.Domain.Features.Transactions;
using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class PayCashflowRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public PayCashflowRequestBuilder ForCashflow(Cashflow cashflow)
    {
        _faker.RuleFor(x => x.CashflowId, cashflow.Id);
        return this;
    }

    public PayCashflowRequestBuilder ForDate(DateTime cashflowDate)
    {
        _faker.RuleFor(x => x.CashflowDate, cashflowDate);
        return this;
    }

    public static PayCashflowRequestBuilder GivenACashflowPayment() => new();

    public static PayCashflowRequestBuilder GivenAnInvalidCashflowPayment()
    {
        PayCashflowRequestBuilder builder = new PayCashflowRequestBuilder();
        builder._faker.RuleFor(x => x.CashflowId, Guid.Empty);
        return builder;
    }
}
