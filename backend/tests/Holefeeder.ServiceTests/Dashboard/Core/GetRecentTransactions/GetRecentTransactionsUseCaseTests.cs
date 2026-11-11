using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;
using Holefeeder.ServiceTests.Dashboard.TestKit;

namespace Holefeeder.ServiceTests.Dashboard.Core.GetRecentTransactions;

public class GetRecentTransactionsUseCaseTests
{
    private readonly DashboardInMemory _dashboard = new();
    private readonly GetRecentTransactionsUseCase _useCase;

    public GetRecentTransactionsUseCaseTests() => _useCase = new GetRecentTransactionsUseCase(_dashboard);

    [Fact]
    public async Task HandleAsync_TitlesATransactionWithItsDescriptionAndShowsTheCategory()
    {
        var user = AUser();
        var lunch = ATransaction(new DateOnly(2026, 11, 8), 12.34m, "Lunch with Sam", "Food", "#FF8D28", ["restaurant", "work"]);
        _dashboard.Add(user, lunch);

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(user, 4), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(
            new DashboardTransaction(lunch.Id, "Lunch with Sam", "Food", "#FF8D28", ["restaurant", "work"], 12.34m, new DateOnly(2026, 11, 8)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_TitlesATransactionWithoutDescriptionWithItsCategory(string description)
    {
        var user = AUser();
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 8), 1000m, description, "Pay check", "#34C759", []));

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(user, 4), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Title = "Pay check", Category = (string?)null });
    }

    [Fact]
    public async Task HandleAsync_TrimsTheDescription()
    {
        var user = AUser();
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 8), 5m, "  Coffee ", "Food", "#FF8D28", []));

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(user, 4), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Title.Should().Be("Coffee");
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheMostRecentTransactionsUpToTheCount()
    {
        var user = AUser();
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 1), 1m, "First", "Food", "#000000", []));
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 2), 2m, "Second", "Food", "#000000", []));
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 5), 5m, "Fifth", "Food", "#000000", []));
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 3), 3m, "Third", "Food", "#000000", []));
        _dashboard.Add(user, ATransaction(new DateOnly(2026, 11, 4), 4m, "Fourth", "Food", "#000000", []));

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(user, 4), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(transaction => transaction.Title).Should().Equal("Fifth", "Fourth", "Third", "Second");
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersTransactions()
    {
        _dashboard.Add(AUser(), ATransaction(new DateOnly(2026, 11, 8), 1m, "Other", "Food", "#000000", []));

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(AUser(), 4), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsDashboardError_WhenDashboardFails()
    {
        var error = new ResultError("dashboard-failed", "Dashboard failed.");
        _dashboard.FailWith(error);

        var result = await _useCase.HandleAsync(new GetRecentTransactionsRequest(AUser(), 4), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static RecentTransaction ATransaction(
        DateOnly date, decimal amount, string description, string categoryName, string categoryColor, string[] tags) =>
        new(AnId(), date, amount, description, categoryName, categoryColor, tags);
}
