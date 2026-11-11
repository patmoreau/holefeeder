using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.ClearCashflow;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;

namespace Holefeeder.ServiceTests.Cashflows.Core.ClearCashflow;

public class ClearCashflowUseCaseTests
{
    private static readonly DateOnly Scheduled = new(2026, 11, 1);

    private readonly CashflowsInMemory _cashflows = new();
    private readonly ClearCashflowUseCase _useCase;

    public ClearCashflowUseCaseTests() => _useCase = new ClearCashflowUseCase(_cashflows);

    [Fact]
    public async Task HandleAsync_RecordsAZeroPaymentOnTheScheduledDate()
    {
        var user = AUser();
        var cashflow = ACashflow(amount: 1450m);
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(new ClearCashflowRequest(user, cashflow.Id, Scheduled), CancellationToken.None);

        result.Should().BeSuccessful();
        _cashflows.PaymentsOf(user).Should().Equal(cashflow.Pay(result.Value, Scheduled, Money.Zero, Scheduled));
        _cashflows.CashflowOf(user, cashflow.Id).Should().Be(cashflow);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowBelongsToAnotherUser()
    {
        var cashflow = ACashflow();
        _cashflows.Add(AUser(), cashflow);
        var user = AUser();

        var result = await _useCase.HandleAsync(new ClearCashflowRequest(user, cashflow.Id, Scheduled), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
        _cashflows.PaymentsOf(user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheStoreError_WhenTheStoreFails()
    {
        var error = new ResultError("cashflows-failed", "Cashflows failed.");
        _cashflows.FailWith(error);

        var result = await _useCase.HandleAsync(new ClearCashflowRequest(AUser(), AnId(), Scheduled), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
