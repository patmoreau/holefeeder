using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Persistence;
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
