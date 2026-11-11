using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.CloseAccount;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.CloseAccount;

public class CloseAccountUseCaseTests
{
    private readonly AccountsInMemory _accounts = new();
    private readonly CloseAccountUseCase _useCase;

    public CloseAccountUseCaseTests() => _useCase = new CloseAccountUseCase(_accounts);

    [Fact]
    public async Task HandleAsync_MakesTheAccountInactive()
    {
        var user = AUser();
        var amex = AnAccount() with { Favorite = true, Description = "Travel card", OpenBalance = 12m };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(new CloseAccountRequest(user, amex.Id), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, amex.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            amex with { Inactive = true });
    }

    [Fact]
    public async Task HandleAsync_FailsWithAlreadyClosed_WhenTheAccountIsInactive()
    {
        var user = AUser();
        var amex = AnAccount() with { Inactive = true };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(new CloseAccountRequest(user, amex.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountClosingErrors.AlreadyClosed);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var amex = AnAccount();
        _accounts.Add(AUser(), amex);

        var result = await _useCase.HandleAsync(new CloseAccountRequest(AUser(), amex.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheAccountsError_WhenTheAccountsFail()
    {
        var user = AUser();
        var amex = AnAccount();
        _accounts.Add(user, amex);
        var error = new ResultError("accounts-failed", "Accounts failed.");
        _accounts.FailWith(error);

        var result = await _useCase.HandleAsync(new CloseAccountRequest(user, amex.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    [Fact]
    public void AlreadyClosed_DescribesTheClosingRule()
    {
        AccountClosingErrors.AlreadyClosed.Code.Should().Be("account-already-closed");
        AccountClosingErrors.AlreadyClosed.Description.Should().Be("Account already closed.");
    }

    private static AccountTotals AnAccount() =>
        new(AnId(), "Amex", AccountType.CreditCard, string.Empty, 0m, new DateOnly(2025, 3, 1), Favorite: false, Inactive: false,
            0m, 0m, null);
}
