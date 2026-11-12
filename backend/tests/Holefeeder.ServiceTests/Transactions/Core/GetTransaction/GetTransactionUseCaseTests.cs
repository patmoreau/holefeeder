using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.GetTransaction;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.GetTransaction;

public class GetTransactionUseCaseTests
{
    private readonly TransactionsInMemory _transactions = new();
    private readonly GetTransactionUseCase _useCase;
    private readonly User _user = AUser();
    private readonly TransactionAccount _chequing = AnAccount();
    private readonly TransactionCategory _groceries = ACategory();
    private readonly Transaction _market;

    public GetTransactionUseCaseTests()
    {
        _useCase = new GetTransactionUseCase(_transactions);
        _transactions.Add(_user, _chequing);
        _transactions.Add(_user, _groceries);
        _market = new Transaction(AnId(), new DateOnly(2026, 8, 28), Money.Valid(123.45m), "Market", _chequing.Id, _groceries.Id,
            TagList.Create(["weekly", "family"]), null, null);
        _transactions.Add(_user, _market);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheTransactionWithItsAccountAndCategory()
    {
        var result = await _useCase.HandleAsync(new GetTransactionRequest(_user, _market.Id), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new TransactionDetail(
            _market.Id, new DateOnly(2026, 8, 28), Money.Valid(123.45m), "Market", TagList.Create(["weekly", "family"]), _chequing, _groceries));
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionIsSomeoneElses()
    {
        var result = await _useCase.HandleAsync(new GetTransactionRequest(AUser(), _market.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTransactionCannotBeRead()
    {
        var error = new UnexpectedError("transactions-read-failed", "Transactions could not be read from the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(new GetTransactionRequest(_user, _market.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
