using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.ModifyTransaction;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.ModifyTransaction;

public class ModifyTransactionUseCaseTests
{
    private static readonly DateOnly Paid = new(2026, 9, 1);

    private readonly TransactionsInMemory _transactions = new();
    private readonly ModifyTransactionUseCase _useCase;
    private readonly User _user = AUser();
    private readonly TransactionAccount _chequing = AnAccount();
    private readonly TransactionAccount _savings = AnAccount("Savings");
    private readonly TransactionCategory _groceries = ACategory();
    private readonly TransactionCategory _transferIn = ACategory("Transfer In", CategoryType.Gain);
    private readonly Id _cashflowId = AnId();
    private readonly Transaction _payment;

    public ModifyTransactionUseCaseTests()
    {
        _useCase = new ModifyTransactionUseCase(_transactions);
        _transactions.Add(_user, _chequing);
        _transactions.Add(_user, _savings);
        _transactions.Add(_user, _groceries);
        _transactions.Add(_user, _transferIn);
        _payment = new Transaction(AnId(), new DateOnly(2026, 8, 28), Money.Valid(123.45m), "Market", _chequing.Id, _groceries.Id,
            Tags.Create(["weekly"]), _cashflowId, new DateOnly(2026, 8, 28));
        _transactions.Add(_user, _payment);
    }

    private ModifyTransactionRequest AModification(
        Id? id = null, Id? accountId = null, Id? categoryId = null, string? description = "From savings") =>
        new(_user, id ?? _payment.Id, Paid, Money.Valid(50m), description, accountId ?? _savings.Id, categoryId ?? _transferIn.Id,
            Tags.Create(["family"]));

    [Fact]
    public async Task HandleAsync_SavesEveryFieldAndKeepsTheCashflowPaid()
    {
        var result = await _useCase.HandleAsync(AModification(), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Should().Equal(new Transaction(
            _payment.Id, Paid, Money.Valid(50m), "From savings", _savings.Id, _transferIn.Id, Tags.Create(["family"]), _cashflowId,
            new DateOnly(2026, 8, 28)));
    }

    [Fact]
    public async Task HandleAsync_KeepsTheDescription_WhenNoneIsGiven()
    {
        var result = await _useCase.HandleAsync(AModification(description: null), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Should().ContainSingle().Which.Description.Should().Be("Market");
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheAccountIsNotOneOfTheUsers()
    {
        var accountId = AnId();

        var result = await _useCase.HandleAsync(AModification(accountId: accountId), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.AccountMissing(accountId));
        _transactions.TransactionsOf(_user).Should().Equal(_payment);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheCategoryIsNotOneOfTheUsers()
    {
        var categoryId = AnId();

        var result = await _useCase.HandleAsync(AModification(categoryId: categoryId), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.CategoryMissing(categoryId));
        _transactions.TransactionsOf(_user).Should().Equal(_payment);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionIsNotOneOfTheUsers()
    {
        var result = await _useCase.HandleAsync(AModification(id: AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.NotFound);
        _transactions.TransactionsOf(_user).Should().Equal(_payment);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionCannotBeRead()
    {
        var error = new UnexpectedError("transactions-read-failed", "Transactions could not be read from the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(AModification(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
