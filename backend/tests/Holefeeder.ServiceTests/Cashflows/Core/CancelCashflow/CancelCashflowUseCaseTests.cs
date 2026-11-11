using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.CancelCashflow;
using Holefeeder.ServiceTests.Cashflows.TestKit;

namespace Holefeeder.ServiceTests.Cashflows.Core.CancelCashflow;

public class CancelCashflowUseCaseTests
{
    private readonly CashflowsInMemory _cashflows = new();
    private readonly CancelCashflowUseCase _useCase;

    public CancelCashflowUseCaseTests() => _useCase = new CancelCashflowUseCase(_cashflows);

    [Fact]
    public async Task HandleAsync_MakesTheCashflowInactiveWithoutAPayment()
    {
        var user = AUser();
        var cashflow = ACashflow();
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(new CancelCashflowRequest(user, cashflow.Id), CancellationToken.None);

        result.Should().BeSuccessful();
        _cashflows.CashflowOf(user, cashflow.Id).Should().Be(cashflow with { Inactive = true });
        _cashflows.PaymentsOf(user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowIsAlreadyInactive()
    {
        var user = AUser();
        var cashflow = ACashflow(inactive: true);
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(new CancelCashflowRequest(user, cashflow.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.AlreadyInactive);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowBelongsToAnotherUser()
    {
        var owner = AUser();
        var cashflow = ACashflow();
        _cashflows.Add(owner, cashflow);

        var result = await _useCase.HandleAsync(new CancelCashflowRequest(AUser(), cashflow.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
        _cashflows.CashflowOf(owner, cashflow.Id).Should().Be(cashflow);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheStoreError_WhenTheStoreFails()
    {
        var error = new ResultError("cashflows-failed", "Cashflows failed.");
        _cashflows.FailWith(error);

        var result = await _useCase.HandleAsync(new CancelCashflowRequest(AUser(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
