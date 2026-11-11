using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.FavoriteAccount;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Accounts.TestKit;

namespace Holefeeder.ServiceTests.Accounts.Core.FavoriteAccount;

public class FavoriteAccountUseCaseTests
{
    private readonly AccountsInMemory _accounts = new();
    private readonly FavoriteAccountUseCase _useCase;

    public FavoriteAccountUseCaseTests() => _useCase = new FavoriteAccountUseCase(_accounts);

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task HandleAsync_SetsTheFavoriteFlag(bool favorite, bool isFavorite)
    {
        var user = AUser();
        var amex = AnAccount() with { Favorite = favorite, Description = "Travel card", Inactive = true };
        _accounts.Add(user, amex);

        var result = await _useCase.HandleAsync(new FavoriteAccountRequest(user, amex.Id, isFavorite), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _accounts.GetAccountTotalsAsync(user, amex.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            amex with { Favorite = isFavorite });
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var amex = AnAccount();
        _accounts.Add(AUser(), amex);

        var result = await _useCase.HandleAsync(new FavoriteAccountRequest(AUser(), amex.Id, true), CancellationToken.None);

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

        var result = await _useCase.HandleAsync(new FavoriteAccountRequest(user, amex.Id, true), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private static AccountTotals AnAccount() =>
        new(AnId(), "Amex", AccountType.CreditCard, string.Empty, 0m, new DateOnly(2025, 3, 1), Favorite: false, Inactive: false,
            0m, 0m, null);
}
