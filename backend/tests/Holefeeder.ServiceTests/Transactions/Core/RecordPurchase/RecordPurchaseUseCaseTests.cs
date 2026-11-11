using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.RecordPurchase;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.RecordPurchase;

public class RecordPurchaseUseCaseTests
{
    private static readonly DateOnly Bought = new(2026, 11, 11);

    private readonly TransactionsInMemory _transactions = new();
    private readonly RecordPurchaseUseCase _useCase;
    private readonly User _user = AUser();
    private readonly TransactionAccount _chequing = AnAccount();
    private readonly TransactionCategory _groceries = ACategory();

    public RecordPurchaseUseCaseTests()
    {
        _useCase = new RecordPurchaseUseCase(_transactions);
        _transactions.Add(_user, _chequing);
        _transactions.Add(_user, _groceries);
    }

    private RecordPurchaseRequest APurchase(Id? accountId = null, Id? categoryId = null, Repeat? repeat = null) =>
        new(_user, Bought, Money.Valid(42.50m), "Market", accountId ?? _chequing.Id, categoryId ?? _groceries.Id,
            Tags.Create(["weekly", "costco"]), repeat);

    [Fact]
    public async Task HandleAsync_RecordsThePurchase()
    {
        var result = await _useCase.HandleAsync(APurchase(), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Should().Equal(new Transaction(
            result.Value, Bought, Money.Valid(42.50m), "Market", _chequing.Id, _groceries.Id, Tags.Create(["weekly", "costco"]), null, null));
        _transactions.CashflowsOf(_user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_CreatesTheCashflowAndPaysItsFirstOccurrence_WhenThePurchaseRepeats()
    {
        var repeat = new Repeat(new DateOnly(2026, 11, 15), IntervalType.Monthly, Frequency.Valid(2));

        var result = await _useCase.HandleAsync(APurchase(repeat: repeat), CancellationToken.None);

        result.Should().BeSuccessful();
        var cashflow = _transactions.CashflowsOf(_user).Should().ContainSingle().Subject;
        cashflow.Should().Be(new Cashflow(
            cashflow.Id, new DateOnly(2026, 11, 15), Money.Valid(42.50m), IntervalType.Monthly, Frequency.Valid(2), "Market",
            _chequing.Id, _groceries.Id, Tags.Create(["weekly", "costco"])));
        _transactions.TransactionsOf(_user).Should().ContainSingle()
            .Which.Should().Match<Transaction>(transaction => transaction.CashflowId == cashflow.Id && transaction.CashflowDate == Bought);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheAccountIsNotOneOfTheUsersOpenAccounts()
    {
        var result = await _useCase.HandleAsync(APurchase(accountId: AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.AccountNotFound);
        _transactions.TransactionsOf(_user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCategoryIsNotOneOfTheUsersCategories()
    {
        var result = await _useCase.HandleAsync(APurchase(categoryId: AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.CategoryNotFound);
        _transactions.TransactionsOf(_user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenThePurchaseCannotBeSaved()
    {
        var error = new UnexpectedError("transactions-write-failed", "Transactions could not be written to the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(APurchase(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
