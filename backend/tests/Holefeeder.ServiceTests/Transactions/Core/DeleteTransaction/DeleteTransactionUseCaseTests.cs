using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.DeleteTransaction;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.DeleteTransaction;

public class DeleteTransactionUseCaseTests
{
    private readonly TransactionsInMemory _transactions = new();
    private readonly DeleteTransactionUseCase _useCase;
    private readonly User _user = AUser();
    private readonly Transaction _market;
    private readonly Transaction _bakery;

    public DeleteTransactionUseCaseTests()
    {
        _useCase = new DeleteTransactionUseCase(_transactions);
        var chequing = AnAccount();
        var groceries = ACategory();
        _market = new Transaction(AnId(), new DateOnly(2026, 8, 28), Money.Valid(10m), "Market", chequing.Id, groceries.Id,
            TagList.Create([]), null, null);
        _bakery = _market with { Id = AnId(), Description = "Bakery" };
        _transactions.Add(_user, _market);
        _transactions.Add(_user, _bakery);
    }

    [Fact]
    public async Task HandleAsync_DeletesTheTransaction()
    {
        var result = await _useCase.HandleAsync(new DeleteTransactionRequest(_user, _market.Id), CancellationToken.None);

        result.Should().BeSuccessful();
        _transactions.TransactionsOf(_user).Should().Equal(_bakery);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionIsSomeoneElses()
    {
        var result = await _useCase.HandleAsync(new DeleteTransactionRequest(AUser(), _market.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.NotFound);
        _transactions.TransactionsOf(_user).Should().Equal(_market, _bakery);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionCannotBeDeleted()
    {
        var error = new UnexpectedError("transactions-write-failed", "Transactions could not be written to the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(new DeleteTransactionRequest(_user, _market.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
