using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;

namespace Holefeeder.ServiceTests.Dashboard.Core.GetDashboardAccounts;

public class GetDashboardAccountsUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly DashboardInMemory _dashboard = new();
    private readonly GetDashboardAccountsUseCase _useCase;

    public GetDashboardAccountsUseCaseTests() => _useCase = new GetDashboardAccountsUseCase(_dashboard);

    [Fact]
    public async Task HandleAsync_ShowsAnAssetBalanceAsAvailable()
    {
        var user = AUser();
        var chequing = AnAccount("Chequing", AccountType.Checking, openBalance: 100m, gains: 2500m, expenses: 613.59m, lastTransaction: new DateOnly(2026, 11, 8));
        _dashboard.Add(user, chequing);

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(
            new DashboardAccount(chequing.Id, "Chequing", BalanceCaption.AvailableBalance, 1986.41m, 1986.41m, new DateOnly(2026, 11, 8)));
    }

    [Fact]
    public async Task HandleAsync_ShowsAnOverdrawnAssetAsDue()
    {
        var user = AUser();
        var chequing = AnAccount("Chequing", AccountType.Checking, openBalance: 0m, gains: 0m, expenses: 508.11m);
        _dashboard.Add(user, chequing);

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Caption = BalanceCaption.AmountDue, Balance = -508.11m });
    }

    [Fact]
    public async Task HandleAsync_ShowsAnOwingLiabilityAsDue()
    {
        var user = AUser();
        var amex = AnAccount("Amex", AccountType.CreditCard, openBalance: 0m, gains: 100m, expenses: 223.45m);
        _dashboard.Add(user, amex);

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Caption = BalanceCaption.AmountDue, Balance = -123.45m });
    }

    [Fact]
    public async Task HandleAsync_ShowsALiabilityInCreditAsCredit()
    {
        var user = AUser();
        var amex = AnAccount("Amex", AccountType.CreditCard, openBalance: 30m, gains: 50m, expenses: 0m);
        _dashboard.Add(user, amex);

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Caption = BalanceCaption.CreditBalance, Balance = 20m });
    }

    [Theory]
    [InlineData("Savings", "AvailableBalance")]
    [InlineData("Mortgage", "AmountDue")]
    public async Task HandleAsync_NamesAZeroBalanceAfterTheAccountsNormalState(string type, string caption)
    {
        var user = AUser();
        _dashboard.Add(user, AnAccount("Account", AccountType.Valid(type), openBalance: 0m, gains: 0.004m, expenses: 0m));

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Caption.Should().Be(Enum.Parse<BalanceCaption>(caption));
    }

    [Fact]
    public async Task HandleAsync_ProjectsUnpaidCashflowsOfTheAccountUntilTheEndOfThePeriod()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        var chequing = AnAccount("Chequing", AccountType.Checking, openBalance: 0m, gains: 2000m, expenses: 0m);
        var amex = AnAccount("Amex", AccountType.CreditCard, openBalance: 0m, gains: 0m, expenses: 123.45m);
        _dashboard.Add(user, chequing);
        _dashboard.Add(user, amex);
        _dashboard.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 11, 15), IntervalType.Monthly, 1, 1450m, CategoryType.Expense, null, null));
        _dashboard.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 11, 13), IntervalType.Weekly, 2, 1000m, CategoryType.Gain, null, null));
        _dashboard.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 12, 1), IntervalType.Monthly, 1, 500m, CategoryType.Expense, null, null));
        _dashboard.Add(user, new CashflowSchedule(amex.Id, new DateOnly(2026, 11, 20), IntervalType.OneTime, 1, 66.67m, CategoryType.Expense, null, null));

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(account => (account.Name, account.Balance, account.ProjectedBalance))
            .Should().Equal(("Chequing", 2000m, 2550m), ("Amex", -123.45m, -190.12m));
    }

    [Fact]
    public async Task HandleAsync_ShowsTheOpenDate_WhenTheAccountHasNoTransactions()
    {
        var user = AUser();
        _dashboard.Add(user, AnAccount("Savings", AccountType.Savings, openBalance: 12450m, gains: 0m, expenses: 0m, lastTransaction: null));

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Updated.Should().Be(OpenDate);
    }

    [Fact]
    public async Task HandleAsync_KeepsTheOrderOfTheAccounts()
    {
        var user = AUser();
        _dashboard.Add(user, AnAccount("Savings", AccountType.Savings, 0m, 0m, 0m));
        _dashboard.Add(user, AnAccount("Amex", AccountType.CreditCard, 0m, 0m, 0m));
        _dashboard.Add(user, AnAccount("Chequing", AccountType.Checking, 0m, 0m, 0m));

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(account => account.Name).Should().Equal("Savings", "Amex", "Chequing");
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersAccounts()
    {
        _dashboard.Add(AUser(), AnAccount("Chequing", AccountType.Checking, 0m, 0m, 0m));

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsDashboardError_WhenDashboardFails()
    {
        var error = new ResultError("dashboard-failed", "Dashboard failed.");
        _dashboard.FailWith(error);

        var result = await _useCase.HandleAsync(new GetDashboardAccountsRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static AccountTotals AnAccount(
        string name, AccountType type, decimal openBalance, decimal gains, decimal expenses, DateOnly? lastTransaction = null) =>
        new(AnId(), name, type, openBalance, OpenDate, gains, expenses, lastTransaction);
}
