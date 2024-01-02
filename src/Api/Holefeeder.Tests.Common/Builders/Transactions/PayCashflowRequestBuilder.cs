using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Transactions;

using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class PayCashflowRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .RuleFor(x => x.Date, faker => faker.Date.SoonDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.CashflowId, faker => faker.Random.Guid())
        .RuleFor(x => x.CashflowDate, faker => faker.Date.SoonDateOnly());

    public PayCashflowRequestBuilder ForCashflow(Cashflow cashflow)
    {
        FakerRules.RuleFor(x => x.CashflowId, cashflow.Id);
        return this;
    }

    public PayCashflowRequestBuilder ForDate(DateOnly cashflowDate)
    {
        FakerRules.RuleFor(x => x.CashflowDate, cashflowDate);
        return this;
    }

    public static PayCashflowRequestBuilder GivenACashflowPayment() => new();

    public static PayCashflowRequestBuilder GivenAnInvalidCashflowPayment()
    {
        var builder = new PayCashflowRequestBuilder();
        builder.FakerRules.RuleFor(x => x.CashflowId, Guid.Empty);
        return builder;
    }
}
