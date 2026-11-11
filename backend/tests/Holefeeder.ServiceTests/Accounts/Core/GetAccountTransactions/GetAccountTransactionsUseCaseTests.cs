using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.GetAccountTransactions;

public class GetAccountTransactionsUseCaseTests
{
    private readonly AccountsInMemory _accounts = new();
    private readonly GetAccountTransactionsUseCase _useCase;

    public GetAccountTransactionsUseCaseTests() => _useCase = new GetAccountTransactionsUseCase(_accounts);

    [Fact]
    public async Task HandleAsync_TitlesEachTransactionByItsDescriptionOrCategory()
    {
        var user = AUser();
        var accountId = AnId();
        var lunch = ARecordedTransaction(accountId, new DateOnly(2026, 9, 13), 1.25m, "  Lunch ", "Food", "#FF8D28", ["restaurant", "work"]);
        var payCheck = ARecordedTransaction(accountId, new DateOnly(2026, 9, 12), 1000m, "", "Pay check", "#34C759", []);
        _accounts.Add(user, accountId, lunch, payCheck);

        var result = await _useCase.HandleAsync(ARequest(user, accountId), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(2);
        result.Value.Items.Should().BeEquivalentTo([
            new AccountTransaction(lunch.Id, "Lunch", "Food", "#FF8D28", ["restaurant", "work"], 1.25m, new DateOnly(2026, 9, 13)),
            new AccountTransaction(payCheck.Id, "Pay check", null, "#34C759", [], 1000m, new DateOnly(2026, 9, 12))
        ], options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheRequestedPageWithTheTotal()
    {
        var user = AUser();
        var accountId = AnId();
        var transactions = Enumerable.Range(1, 5)
            .Select(day => ARecordedTransaction(accountId, new DateOnly(2026, 9, 6 - day), day, "", "Food", "#FF8D28", []))
            .ToArray();
        _accounts.Add(user, accountId, transactions);

        var result = await _useCase.HandleAsync(ARequest(user, accountId, offset: 2, limit: 2), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(5);
        result.Value.Items.Select(transaction => transaction.Amount).Should().Equal(3m, 4m);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNoTransaction_WhenTheAccountHasNone()
    {
        var user = AUser();
        var accountId = AnId();
        _accounts.Add(user, accountId);

        var result = await _useCase.HandleAsync(ARequest(user, accountId), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(0);
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var accountId = AnId();
        _accounts.Add(AUser(), accountId);

        var result = await _useCase.HandleAsync(ARequest(AUser(), accountId), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheAccountsError_WhenTheAccountsFail()
    {
        var error = new ResultError("accounts-failed", "Accounts failed.");
        _accounts.FailWith(error);

        var result = await _useCase.HandleAsync(ARequest(AUser(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static GetAccountTransactionsRequest ARequest(Service.Support.Core.User user, Id accountId, int offset = 0, int limit = 20) =>
        new(user, accountId, Offset.Valid(offset), Limit.Valid(limit));

    private static RecordedTransaction ARecordedTransaction(
        Id accountId, DateOnly date, decimal amount, string description, string categoryName, string categoryColor, string[] tags) =>
        new(AnId(), accountId, date, amount, description, categoryName, categoryColor, tags);
}
