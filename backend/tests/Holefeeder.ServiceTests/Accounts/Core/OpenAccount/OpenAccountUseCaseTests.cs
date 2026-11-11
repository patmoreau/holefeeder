using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.OpenAccount;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.OpenAccount;

public class OpenAccountUseCaseTests
{
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly AccountsInMemory _accounts = new();
    private readonly OpenAccountUseCase _useCase;

    public OpenAccountUseCaseTests() => _useCase = new OpenAccountUseCase(_accounts);

    [Fact]
    public async Task HandleAsync_OpensAnActiveAccountThatIsNotAFavorite()
    {
        var user = AUser();

        var result = await _useCase.HandleAsync(
            new OpenAccountRequest(user, AccountType.Savings, AccountName.Valid("Savings"), OpenDate, Money.Valid(12450m), "Rainy days"),
            CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, result.Value, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            new AccountTotals(result.Value, "Savings", AccountType.Savings, "Rainy days", 12450m, OpenDate, Favorite: false, Inactive: false,
                Gains: 0m, Expenses: 0m, LastTransactionDate: null));
    }

    [Fact]
    public async Task HandleAsync_OpensTheAccount_WhenAnotherUserHasTheName()
    {
        _accounts.Add(AUser(), AnAccount("Chequing"));

        var result = await _useCase.HandleAsync(AnOpenAccountRequest(AUser(), "Chequing"), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_FailsWithNameAlreadyExists_WhenTheUserHasTheName()
    {
        var user = AUser();
        _accounts.Add(user, AnAccount("Chequing"));

        var result = await _useCase.HandleAsync(AnOpenAccountRequest(user, "Chequing"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountNameErrors.AlreadyExists);
    }

    [Fact]
    public async Task HandleAsync_FailsWithUserNotRegistered_WhenThereIsNoUser()
    {
        var result = await _useCase.HandleAsync(AnOpenAccountRequest(User.NoUser, "Chequing"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UserErrors.NotRegistered);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheAccountsError_WhenTheAccountsFail()
    {
        var error = new ResultError("accounts-failed", "Accounts failed.");
        _accounts.FailWith(error);

        var result = await _useCase.HandleAsync(AnOpenAccountRequest(AUser(), "Chequing"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static OpenAccountRequest AnOpenAccountRequest(User user, string name) =>
        new(user, AccountType.Checking, AccountName.Valid(name), OpenDate, Money.Zero, string.Empty);

    private static AccountTotals AnAccount(string name) =>
        new(AnId(), name, AccountType.Checking, string.Empty, 0m, OpenDate, Favorite: false, Inactive: false, 0m, 0m, null);
}
