using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.ModifyAccount;

public class ModifyAccountUseCaseTests
{
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly AccountsInMemory _accounts = new();
    private readonly ModifyAccountUseCase _useCase;

    public ModifyAccountUseCaseTests() => _useCase = new ModifyAccountUseCase(_accounts);

    [Fact]
    public async Task HandleAsync_ChangesTheNameOpenBalanceAndDescription()
    {
        var user = AUser();
        var amex = AnAccount("Amex") with { Favorite = true, Inactive = true, Gains = 30m, Expenses = 10m };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Amex Gold"), null, null, Money.Valid(99.5m), "Travel card"),
            CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, amex.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            amex with { Name = "Amex Gold", OpenBalance = 99.5m, Description = "Travel card" });
    }

    [Fact]
    public async Task HandleAsync_ChangesTheTypeAndOpenDate()
    {
        var user = AUser();
        var amex = AnAccount("Amex") with { OpenBalance = 12m, Description = "Travel card" };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Amex"), AccountType.CreditLine, new DateOnly(2024, 6, 15), null, null),
            CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, amex.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            amex with { Type = AccountType.CreditLine, OpenDate = new DateOnly(2024, 6, 15) });
    }

    [Fact]
    public async Task HandleAsync_KeepsTheTypeOpenDateOpenBalanceAndDescription_WhenTheyAreMissing()
    {
        var user = AUser();
        var amex = AnAccount("Amex") with { OpenBalance = 12m, Description = "Travel card" };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Amex Gold"), null, null, null, null), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, amex.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            amex with { Name = "Amex Gold" });
    }

    [Fact]
    public async Task HandleAsync_KeepsTheName_WhenTheAccountAlreadyHasIt()
    {
        var user = AUser();
        var amex = AnAccount("Amex");
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Amex"), null, null, Money.Valid(5m), null), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameAlreadyExists_WhenAnotherAccountOfTheUserHasTheName()
    {
        var user = AUser();
        var amex = AnAccount("Amex");
        _accounts.Add(user, amex);
        _accounts.Add(user, AnAccount("Visa"));

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Visa"), null, null, null, null), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountNameErrors.AlreadyExists);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var amex = AnAccount("Amex");
        _accounts.Add(AUser(), amex);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(AUser(), amex.Id, AccountName.Valid("Mine now"), null, null, null, null), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheAccountsError_WhenTheAccountsFail()
    {
        var user = AUser();
        var amex = AnAccount("Amex");
        _accounts.Add(user, amex);
        var error = new ResultError("accounts-failed", "Accounts failed.");
        _accounts.FailWith(error);

        var result = await _useCase.HandleAsync(
            new ModifyAccountRequest(user, amex.Id, AccountName.Valid("Amex Gold"), null, null, null, null), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static AccountTotals AnAccount(string name) =>
        new(AnId(), name, AccountType.CreditCard, string.Empty, 0m, OpenDate, Favorite: false, Inactive: false, 0m, 0m, null);
}
