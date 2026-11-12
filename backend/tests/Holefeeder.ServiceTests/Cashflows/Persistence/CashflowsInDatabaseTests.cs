using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Cashflows.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Cashflows.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class CashflowsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Scheduled = new(2026, 11, 1);

    private readonly CashflowsInDatabase _repository = new(database.DataSource, new LoggerForTest<CashflowsInDatabase>(testOutputHelper));

    [Fact]
    public async Task ListAsync_ListsTheActiveCashflowsByTypeThenTitle()
    {
        var user = await AUser().SavedInDbAsync(database);
        var chequing = AnId();
        var housing = AnId();
        var rent = await ACashflow(1450m, description: "Rent", tags: "home,rent", accountId: chequing, categoryId: housing)
            .SavedInDbAsync(user, database, "Housing", "Expense", "#5856D6", "Chequing");
        var gym = await ACashflow(45m, description: "", tags: "", accountId: chequing, effectiveDate: new DateOnly(2026, 11, 2), intervalType: "Weekly", frequency: 2)
            .SavedInDbAsync(user, database, "Health", "Expense", "#30B0C7", "Chequing");
        var payroll = await ACashflow(2400m, description: "Payroll", tags: "", accountId: chequing)
            .SavedInDbAsync(user, database, "Salary", "Gain", "#34C759", "Chequing");
        await _repository.SaveAsync(user, rent, rent.Pay(AnId(), new DateOnly(2026, 2, 2), rent.Amount, new DateOnly(2026, 2, 1)), CancellationToken.None);
        await ACashflow(description: "Old", inactive: true).SavedInDbAsync(user, database);
        await ACashflow(description: "Someone else's").SavedInDbAsync(await AUser().SavedInDbAsync(database), database);

        var result = await _repository.ListAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(
            [
                new CashflowSummary(
                    gym.Id, "", new CashflowSchedule(chequing, new DateOnly(2026, 11, 2), IntervalType.Weekly, 2, 45m, CategoryType.Expense, null, null),
                    "Health", "#30B0C7", "Chequing", []),
                new CashflowSummary(
                    rent.Id, "Rent",
                    new CashflowSchedule(chequing, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m, CategoryType.Expense, new DateOnly(2026, 2, 2), new DateOnly(2026, 2, 1)),
                    "Housing", "#5856D6", "Chequing", ["home", "rent"]),
                new CashflowSummary(
                    payroll.Id, "Payroll", new CashflowSchedule(chequing, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 2400m, CategoryType.Gain, null, null),
                    "Salary", "#34C759", "Chequing", [])
            ],
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task ListAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CashflowsInDatabase(database.BadDataSource, new LoggerForTest<CashflowsInDatabase>(testOutputHelper));

        var result = await repository.ListAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetAsync_ReturnsTheUsersCashflow()
    {
        var user = await AUser().SavedInDbAsync(database);
        var cashflow = await ACashflow(amount: 88.40m, tags: "utilities,home").SavedInDbAsync(user, database);

        var result = await _repository.GetAsync(user, cashflow.Id, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(cashflow);
    }

    [Fact]
    public async Task GetAsync_Fails_WhenTheCashflowBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var cashflow = await ACashflow().SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetAsync(user, cashflow.Id, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
    }

    [Fact]
    public async Task GetAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CashflowsInDatabase(database.BadDataSource, new LoggerForTest<CashflowsInDatabase>(testOutputHelper));

        var result = await repository.GetAsync(AUser(), AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowsErrors.ReadFailed);
    }

    [Fact]
    public async Task SaveAsync_UpdatesTheCashflowAndRecordsThePayment()
    {
        var user = await AUser().SavedInDbAsync(database);
        var cashflow = await ACashflow(amount: 1450m).SavedInDbAsync(user, database);
        var payment = cashflow.Pay(AnId(), new DateOnly(2026, 11, 3), Money.Valid(1500m), Scheduled);
        var updated = cashflow with { Amount = Money.Valid(1500m) };

        var result = await _repository.SaveAsync(user, updated, payment, CancellationToken.None);

        result.Should().BeSuccessful();
        (await database.CashflowInDbAsync(cashflow.Id)).Should().Be(updated);
        (await database.PaymentsInDbAsync(cashflow.Id)).Should().Equal(payment);
    }

    [Fact]
    public async Task SaveAsync_OnlyUpdatesTheCashflow_WhenThereIsNoPayment()
    {
        var user = await AUser().SavedInDbAsync(database);
        var cashflow = await ACashflow().SavedInDbAsync(user, database);
        var cancelled = cashflow with { Inactive = true };

        var result = await _repository.SaveAsync(user, cancelled, null, CancellationToken.None);

        result.Should().BeSuccessful();
        (await database.CashflowInDbAsync(cashflow.Id)).Should().Be(cancelled);
        (await database.PaymentsInDbAsync(cashflow.Id)).Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_ChangesNothing_WhenTheCashflowBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var cashflow = await ACashflow().SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);
        var payment = cashflow.Pay(AnId(), Scheduled, Money.Valid(1m), Scheduled);

        var result = await _repository.SaveAsync(user, cashflow with { Inactive = true }, payment, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowErrors.NotFound);
        (await database.CashflowInDbAsync(cashflow.Id)).Should().Be(cashflow);
        (await database.PaymentsInDbAsync(cashflow.Id)).Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new CashflowsInDatabase(database.BadDataSource, new LoggerForTest<CashflowsInDatabase>(testOutputHelper));

        var result = await repository.SaveAsync(AUser(), ACashflow(), null, CancellationToken.None);

        result.Should().BeFailure().And.WithError(CashflowsErrors.WriteFailed);
    }
}
