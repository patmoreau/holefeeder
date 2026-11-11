using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.PayCashflow;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;

namespace Holefeeder.ServiceTests.Cashflows.Core.PayCashflow;

public class PayCashflowUseCaseTests
{
    private static readonly DateOnly Scheduled = new(2026, 11, 1);
    private static readonly DateOnly PaidOn = new(2026, 11, 3);

    private readonly CashflowsInMemory _cashflows = new();
    private readonly PayCashflowUseCase _useCase;

    public PayCashflowUseCaseTests() => _useCase = new PayCashflowUseCase(_cashflows);

    [Fact]
    public async Task HandleAsync_RecordsThePaymentAndReturnsItsId()
    {
        var user = AUser();
        var cashflow = ACashflow(amount: 1450m);
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(
            new PayCashflowRequest(user, cashflow.Id, PaidOn, Money.Valid(1500m), Scheduled, UpdateRecurringAmount: false),
            CancellationToken.None);

        result.Should().BeSuccessful();
        _cashflows.PaymentsOf(user).Should().Equal(
            cashflow.Pay(result.Value, PaidOn, Money.Valid(1500m), Scheduled));
    }

    [Fact]
    public async Task HandleAsync_KeepsTheRecurringAmount_WhenNotAskedToUpdateIt()
    {
        var user = AUser();
        var cashflow = ACashflow(amount: 1450m);
        _cashflows.Add(user, cashflow);

        await _useCase.HandleAsync(
            new PayCashflowRequest(user, cashflow.Id, PaidOn, Money.Valid(1500m), Scheduled, UpdateRecurringAmount: false),
            CancellationToken.None);

        _cashflows.CashflowOf(user, cashflow.Id)!.Amount.Should().Be(Money.Valid(1450m));
    }

    [Fact]
    public async Task HandleAsync_UpdatesTheRecurringAmount_WhenAsked()
    {
        var user = AUser();
        var cashflow = ACashflow(amount: 1450m);
        _cashflows.Add(user, cashflow);

        await _useCase.HandleAsync(
            new PayCashflowRequest(user, cashflow.Id, PaidOn, Money.Valid(1500m), Scheduled, UpdateRecurringAmount: true),
            CancellationToken.None);

        _cashflows.CashflowOf(user, cashflow.Id)!.Amount.Should().Be(Money.Valid(1500m));
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowBelongsToAnotherUser()
    {
        var cashflow = ACashflow();
        _cashflows.Add(AUser(), cashflow);
        var user = AUser();

        var result = await _useCase.HandleAsync(
            new PayCashflowRequest(user, cashflow.Id, PaidOn, Money.Valid(1450m), Scheduled, UpdateRecurringAmount: false),
            CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
        _cashflows.PaymentsOf(user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheStoreError_WhenSavingFails()
    {
        var error = new ResultError("cashflows-failed", "Cashflows failed.");
        _cashflows.FailWith(error);

        var result = await _useCase.HandleAsync(
            new PayCashflowRequest(AUser(), AnId(), PaidOn, Money.Valid(1450m), Scheduled, UpdateRecurringAmount: false),
            CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
