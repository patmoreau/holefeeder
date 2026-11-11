using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.RecordTransfer;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.RecordTransfer;

public class RecordTransferUseCaseTests
{
    private static readonly DateOnly Moved = new(2026, 11, 11);

    private readonly TransactionsInMemory _transactions = new();
    private readonly RecordTransferUseCase _useCase;
    private readonly User _user = AUser();
    private readonly TransactionAccount _chequing = AnAccount("Chequing");
    private readonly TransactionAccount _savings = AnAccount("Savings");
    private readonly TransferCategories _categories = new(AnId(), AnId());

    public RecordTransferUseCaseTests()
    {
        _useCase = new RecordTransferUseCase(_transactions);
        _transactions.Add(_user, _chequing);
        _transactions.Add(_user, _savings);
        _transactions.Add(_user, _categories);
    }

    private RecordTransferRequest ATransfer(string description = "Rainy day", Id? fromAccountId = null, Id? toAccountId = null) =>
        new(_user, Moved, Money.Valid(500m), description, fromAccountId ?? _chequing.Id, toAccountId ?? _savings.Id);

    [Fact]
    public async Task HandleAsync_TakesTheMoneyOutOfOneAccountAndIntoTheOther()
    {
        var result = await _useCase.HandleAsync(ATransfer(), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Should().Equal(
            new Transaction(result.Value.FromTransactionId, Moved, Money.Valid(500m), "Rainy day", _chequing.Id, _categories.TransferOut,
                Tags.Create([]), null, null),
            new Transaction(result.Value.ToTransactionId, Moved, Money.Valid(500m), "Rainy day", _savings.Id, _categories.TransferIn,
                Tags.Create([]), null, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_DescribesTheTransfer_WhenNoDescriptionIsGiven(string description)
    {
        var result = await _useCase.HandleAsync(ATransfer(description), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Select(transaction => transaction.Description)
            .Should().Equal("Transfer from 'Chequing' to 'Savings'", "Transfer from 'Chequing' to 'Savings'");
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenBothAccountsAreTheSame()
    {
        var result = await _useCase.HandleAsync(ATransfer(toAccountId: _chequing.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.SameAccount);
        _transactions.TransactionsOf(_user).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheSourceAccountIsNotOneOfTheUsersOpenAccounts()
    {
        var result = await _useCase.HandleAsync(ATransfer(fromAccountId: AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheDestinationAccountIsNotOneOfTheUsersOpenAccounts()
    {
        var result = await _useCase.HandleAsync(ATransfer(toAccountId: AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheUserHasNoTransferCategories()
    {
        var user = AUser();
        _transactions.Add(user, _chequing);
        _transactions.Add(user, _savings);

        var result = await _useCase.HandleAsync(ATransfer() with { User = user }, CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.TransferCategoriesNotFound);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransferCannotBeSaved()
    {
        var error = new UnexpectedError("transactions-write-failed", "Transactions could not be written to the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(ATransfer(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
