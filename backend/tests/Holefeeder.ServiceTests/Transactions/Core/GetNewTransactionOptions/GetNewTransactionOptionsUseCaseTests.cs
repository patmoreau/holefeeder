using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core.GetNewTransactionOptions;
using Holefeeder.ServiceTests.Transactions.TestKit;

using static Holefeeder.ServiceTests.Transactions.TestKit.TransactionsForTest;

namespace Holefeeder.ServiceTests.Transactions.Core.GetNewTransactionOptions;

public class GetNewTransactionOptionsUseCaseTests
{
    private readonly TransactionsInMemory _transactions = new();
    private readonly GetNewTransactionOptionsUseCase _useCase;

    public GetNewTransactionOptionsUseCaseTests() => _useCase = new GetNewTransactionOptionsUseCase(_transactions);

    [Fact]
    public async Task HandleAsync_ListsAccountsFavouritesFirstThenByName()
    {
        var user = AUser();
        var savings = AnAccount("Savings");
        var amex = AnAccount("amex");
        var visa = AnAccount("Visa", favorite: true);
        var chequing = AnAccount("Chequing", favorite: true);
        foreach (var account in new[] { savings, amex, visa, chequing })
        {
            _transactions.Add(user, account);
        }

        var result = await _useCase.HandleAsync(new GetNewTransactionOptionsRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Accounts.Should().Equal(chequing, visa, amex, savings);
    }

    [Fact]
    public async Task HandleAsync_ListsCategoriesFavouritesFirstThenByName()
    {
        var user = AUser();
        var salary = ACategory("Salary", CategoryType.Gain);
        var dining = ACategory("dining");
        var groceries = ACategory("Groceries", favorite: true);
        var bonus = ACategory("Bonus", CategoryType.Gain, favorite: true);
        foreach (var category in new[] { salary, dining, groceries, bonus })
        {
            _transactions.Add(user, category);
        }

        var result = await _useCase.HandleAsync(new GetNewTransactionOptionsRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Categories.Should().Equal(bonus, groceries, dining, salary);
    }

    [Fact]
    public async Task HandleAsync_ListsNothing_WhenTheUserHasNoAccountOrCategory()
    {
        _transactions.Add(AUser(), AnAccount());
        _transactions.Add(AUser(), ACategory());

        var result = await _useCase.HandleAsync(new GetNewTransactionOptionsRequest(AUser()), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Accounts.Should().BeEmpty();
        result.Value.Categories.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheOptionsCannotBeRead()
    {
        var error = new UnexpectedError("transactions-read-failed", "Transactions could not be read from the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(new GetNewTransactionOptionsRequest(AUser()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
