using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.ModifyCashflow;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;

namespace Holefeeder.ServiceTests.Cashflows.Core.ModifyCashflow;

public class ModifyCashflowUseCaseTests
{
    private readonly CashflowsInMemory _cashflows = new();
    private readonly ModifyCashflowUseCase _useCase;

    public ModifyCashflowUseCaseTests() => _useCase = new ModifyCashflowUseCase(_cashflows);

    [Fact]
    public async Task HandleAsync_SavesTheModifiedCashflowWithoutAPayment()
    {
        var user = AUser();
        var cashflow = ACashflow(1450m, description: "Rent", tags: "housing");
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(
            new ModifyCashflowRequest(user, cashflow.Id, Money.Valid(1500m), new DateOnly(2026, 2, 1), "New rent", TagList.Create(["Home"])),
            CancellationToken.None);

        result.Should().BeSuccessful();
        _cashflows.CashflowOf(user, cashflow.Id).Should().Be(cashflow with
        {
            Amount = Money.Valid(1500m),
            EffectiveDate = new DateOnly(2026, 2, 1),
            Description = "New rent",
            Tags = "home"
        });
        _cashflows.PaymentsOf(user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowIsCancelled()
    {
        var user = AUser();
        var cashflow = ACashflow(inactive: true);
        _cashflows.Add(user, cashflow);

        var result = await _useCase.HandleAsync(AModification(user, cashflow.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.Cancelled);
        _cashflows.CashflowOf(user, cashflow.Id).Should().Be(cashflow);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCashflowBelongsToAnotherUser()
    {
        var cashflow = ACashflow();
        _cashflows.Add(AUser(), cashflow);

        var result = await _useCase.HandleAsync(AModification(AUser(), cashflow.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenCashflowsCannotBeRead()
    {
        var failure = new ResultError("cashflows-unavailable", "Cashflows are unavailable.");
        _cashflows.FailWith(failure);

        var result = await _useCase.HandleAsync(AModification(AUser(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }

    private static ModifyCashflowRequest AModification(Service.Support.Core.User user, Id id) =>
        new(user, id, Money.Valid(1500m), new DateOnly(2026, 2, 1), "Rent", TagList.Create([]));
}
