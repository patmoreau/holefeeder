using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.GetAccountDetail;

public class GetAccountDetailUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly AccountsInMemory _accounts = new();
    private readonly GetAccountDetailUseCase _useCase;

    public GetAccountDetailUseCaseTests() => _useCase = new GetAccountDetailUseCase(_accounts);

    [Fact]
    public async Task HandleAsync_DescribesTheAccountWithItsBalance()
    {
        var user = AUser();
        var chequing = AnAccount("Chequing", AccountType.Checking, openBalance: 100m, gains: 2500m, expenses: 613.59m,
            lastTransaction: new DateOnly(2026, 11, 8)) with
        {
            Description = "Main bank account",
            Favorite = true
        };
        _accounts.Add(user, chequing);

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(user, chequing.Id, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Be(new AccountDetail(
            chequing.Id, "Chequing", AccountType.Checking, "Main bank account", 100m, OpenDate, Favorite: true, Inactive: false,
            BalanceCaption.AvailableBalance, Balance: 1986.41m, Upcoming: 0m, ProjectedBalance: 1986.41m, Updated: new DateOnly(2026, 11, 8)));
    }

    public static TheoryData<string, decimal, decimal, decimal, string, decimal> Balances => new()
    {
        { "Checking", 0m, 0m, 508.11m, "AmountDue", -508.11m },
        { "CreditCard", 0m, 100m, 223.45m, "AmountDue", -123.45m },
        { "CreditCard", 30m, 50m, 0m, "CreditBalance", 20m },
        { "Savings", 0m, 0.004m, 0m, "AvailableBalance", 0.004m },
        { "Mortgage", 0m, 0.004m, 0m, "AmountDue", 0.004m }
    };

    [Theory]
    [MemberData(nameof(Balances))]
    public async Task HandleAsync_NamesTheBalanceFromTheOwnersPointOfView(
        string type, decimal openBalance, decimal gains, decimal expenses, string caption, decimal balance)
    {
        var user = AUser();
        var account = AnAccount("Account", AccountType.Valid(type), openBalance, gains, expenses);
        _accounts.Add(user, account);

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(user, account.Id, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new { Caption = Enum.Parse<BalanceCaption>(caption), Balance = balance });
    }

    [Fact]
    public async Task HandleAsync_AddsTheUnpaidCashflowsOfTheAccountUntilTheEndOfThePeriod()
    {
        var user = AUser();
        _accounts.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        var chequing = AnAccount("Chequing", AccountType.Checking, openBalance: 0m, gains: 2000m, expenses: 0m);
        var amex = AnAccount("Amex", AccountType.CreditCard, openBalance: 0m, gains: 0m, expenses: 0m);
        _accounts.Add(user, chequing);
        _accounts.Add(user, amex);
        _accounts.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 11, 15), IntervalType.Monthly, 1, 1450m, CategoryType.Expense, null, null));
        _accounts.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 11, 13), IntervalType.Weekly, 2, 1000m, CategoryType.Gain, null, null));
        _accounts.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 12, 1), IntervalType.Monthly, 1, 500m, CategoryType.Expense, null, null));
        _accounts.Add(user, new CashflowSchedule(chequing.Id, new DateOnly(2026, 10, 15), IntervalType.Monthly, 1, 80m, CategoryType.Expense,
            new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 15)));
        _accounts.Add(user, new CashflowSchedule(amex.Id, new DateOnly(2026, 11, 20), IntervalType.OneTime, 1, 66.67m, CategoryType.Expense, null, null));

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(user, chequing.Id, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new { Balance = 2000m, Upcoming = 470m, ProjectedBalance = 2470m });
    }

    [Fact]
    public async Task HandleAsync_ShowsTheOpenDate_WhenTheAccountHasNoTransactions()
    {
        var user = AUser();
        var savings = AnAccount("Savings", AccountType.Savings, openBalance: 12450m, gains: 0m, expenses: 0m, lastTransaction: null);
        _accounts.Add(user, savings);

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(user, savings.Id, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Updated.Should().Be(OpenDate);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var account = AnAccount("Chequing", AccountType.Checking, 0m, 0m, 0m);
        _accounts.Add(AUser(), account);

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(AUser(), account.Id, Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheAccountsError_WhenTheAccountsFail()
    {
        var user = AUser();
        var account = AnAccount("Chequing", AccountType.Checking, 0m, 0m, 0m);
        _accounts.Add(user, account);
        var error = new ResultError("accounts-failed", "Accounts failed.");
        _accounts.FailWith(error);

        var result = await _useCase.HandleAsync(new GetAccountDetailRequest(user, account.Id, Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static AccountTotals AnAccount(
        string name, AccountType type, decimal openBalance, decimal gains, decimal expenses, DateOnly? lastTransaction = null) =>
        new(AnId(), name, type, string.Empty, openBalance, OpenDate, Favorite: false, Inactive: false, gains, expenses, lastTransaction);
}
