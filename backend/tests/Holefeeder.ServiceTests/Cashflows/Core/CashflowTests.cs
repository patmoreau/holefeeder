using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Cashflows.Core;

public class CashflowTests
{
    [Fact]
    public void Pay_RecordsAPaymentWithTheCashflowsAccountCategoryDescriptionAndTags()
    {
        var cashflow = ACashflow(description: "Rent", tags: "housing,home");
        var paymentId = AnId();

        var payment = cashflow.Pay(paymentId, new DateOnly(2026, 11, 3), Money.Valid(1500m), new DateOnly(2026, 11, 1));

        payment.Should().Be(new Payment(
            paymentId, new DateOnly(2026, 11, 3), Money.Valid(1500m), "Rent", cashflow.AccountId, cashflow.CategoryId,
            cashflow.Id, new DateOnly(2026, 11, 1), "housing,home"));
    }

    [Fact]
    public void Cancel_MakesTheCashflowInactive()
    {
        var cashflow = ACashflow();

        var result = cashflow.Cancel();

        result.Should().BeSuccessful().And.WithValue(cashflow with { Inactive = true });
    }

    [Fact]
    public void Cancel_Fails_WhenTheCashflowIsAlreadyInactive() =>
        ACashflow(inactive: true).Cancel().Should().BeFailure().And.WithError(CashflowErrors.AlreadyInactive);

    [Fact]
    public void Modify_ChangesTheAmountDateDescriptionAndTags()
    {
        var cashflow = ACashflow(1450m, description: "Rent", tags: "housing");

        var result = cashflow.Modify(Money.Valid(1500m), new DateOnly(2026, 2, 1), "New rent", Tags.Create(["home", "rent"]));

        result.Should().BeSuccessful().And.WithValue(cashflow with
        {
            Amount = Money.Valid(1500m),
            EffectiveDate = new DateOnly(2026, 2, 1),
            Description = "New rent",
            Tags = "home,rent"
        });
    }

    [Fact]
    public void Modify_KeepsTheDescription_WhenNoneIsGiven()
    {
        var cashflow = ACashflow(description: "Rent");

        var result = cashflow.Modify(Money.Valid(1500m), new DateOnly(2026, 2, 1), null, Tags.Create([]));

        result.Should().BeSuccessful();
        result.Value.Description.Should().Be("Rent");
        result.Value.Tags.Should().BeEmpty();
    }

    [Fact]
    public void Modify_Fails_WhenTheCashflowIsCancelled() =>
        ACashflow(inactive: true).Modify(Money.Valid(1500m), new DateOnly(2026, 2, 1), "Rent", Tags.Create([]))
            .Should().BeFailure().And.WithError(CashflowErrors.Cancelled);

    [Fact]
    public void Errors_FollowTheServiceConventions()
    {
        CashflowErrors.Cancelled.Code.Should().Be("cashflow-cancelled");
        CashflowErrors.Cancelled.Description.Should().Be("A cancelled cashflow cannot be changed.");
        CashflowErrors.NotFound.Should().Be(new NotFoundError("cashflow-not-found", "Cashflow was not found."));
        CashflowErrors.AlreadyInactive.Code.Should().Be("cashflow-already-inactive");
        CashflowErrors.AlreadyInactive.Description.Should().Be("Cashflow is already inactive.");
    }
}
