using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Api;
using Holefeeder.Service.Cashflows.Api.PayCashflow;
using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.PayCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Cashflows.Api.PayCashflow;

public class PayCashflowRequestRawTests
{
    private static PayCashflowBody ABody(
        string? cashflowId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301", string? date = "2026-11-03", string? amount = "1500.00",
        string? cashflowDate = "2026-11-01", string? updateRecurringAmount = null) =>
        new(cashflowId, date, amount, cashflowDate, updateRecurringAmount);

    [Theory]
    [InlineData(null, false)]
    [InlineData("false", false)]
    [InlineData("true", true)]
    public void ToPayCashflowRequest_ReturnsThePaymentForTheUser_WhenValid(string? updateRaw, bool update)
    {
        var user = AUser();

        var result = PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(updateRecurringAmount: updateRaw)), user);

        result.Should().BeSuccessful().And.WithValue(new PayCashflowRequest(
            user, Id.Valid(Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301")), new DateOnly(2026, 11, 3), Money.Valid(1500m),
            new DateOnly(2026, 11, 1), update));
    }

    [Fact]
    public void ToPayCashflowRequest_FailsValidation_WhenCashflowIdIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(cashflowId: "nope")), AUser()))
            .Should().ContainKey(IdErrors.CodeInvalid);

    [Fact]
    public void ToPayCashflowRequest_FailsValidation_WhenDateIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(date: "soon")), AUser()))
            .Should().ContainKey(IsoDateErrors.CodeDateInvalid);

    [Fact]
    public void ToPayCashflowRequest_FailsValidation_WhenAmountIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(amount: "-5")), AUser()))
            .Should().ContainKey(MoneyErrors.CodeInvalid);

    [Fact]
    public void ToPayCashflowRequest_FailsValidation_WhenCashflowDateIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(cashflowDate: null)), AUser()))
            .Should().ContainKey(IsoDateErrors.CodeCashflowDateInvalid);

    [Fact]
    public void ToPayCashflowRequest_FailsValidation_WhenUpdateRecurringAmountIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(ABody(updateRecurringAmount: "maybe")), AUser()))
            .Should().ContainKey(PayCashflowRequestErrors.CodeUpdateRecurringAmountInvalid);

    [Fact]
    public void ToPayCashflowRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(PayCashflowRequestRaw.ToPayCashflowRequest(new(new PayCashflowBody(null, null, null, null, "maybe")), AUser()))
            .Should().ContainKeys(
                IdErrors.CodeInvalid, IsoDateErrors.CodeDateInvalid, MoneyErrors.CodeInvalid, IsoDateErrors.CodeCashflowDateInvalid,
                PayCashflowRequestErrors.CodeUpdateRecurringAmountInvalid);

    [Fact]
    public void Errors_FollowTheServiceConventions()
    {
        PayCashflowRequestErrors.UpdateRecurringAmountInvalid.Code.Should().Be("update-recurring-amount-invalid");
        PayCashflowRequestErrors.UpdateRecurringAmountInvalid.Description.Should().Be("Update recurring amount must be true or false.");
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<PayCashflowRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
