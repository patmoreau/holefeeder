using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.GetTagSuggestions;
using Holefeeder.ServiceTests.Transactions.TestKit;

namespace Holefeeder.ServiceTests.Transactions.Core.GetTagSuggestions;

public class GetTagSuggestionsUseCaseTests
{
    private readonly TransactionsInMemory _transactions = new();
    private readonly GetTagSuggestionsUseCase _useCase;

    public GetTagSuggestionsUseCaseTests() => _useCase = new GetTagSuggestionsUseCase(_transactions);

    [Fact]
    public async Task HandleAsync_SuggestsTagsUsedWithTheCategoryFirst()
    {
        var user = AUser();
        var groceries = AnId();
        var dining = AnId();
        _transactions.Add(user, new TagUsage("family", dining, 9));
        _transactions.Add(user, new TagUsage("costco", groceries, 2));
        _transactions.Add(user, new TagUsage("weekly", groceries, 5));
        _transactions.Add(user, new TagUsage("weekly", dining, 1));

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(user, groceries), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal("weekly", "costco", "family");
    }

    [Fact]
    public async Task HandleAsync_ThenSuggestsTheMostUsedTagsOverall()
    {
        var user = AUser();
        var groceries = AnId();
        var dining = AnId();
        var travel = AnId();
        _transactions.Add(user, new TagUsage("costco", groceries, 3));
        _transactions.Add(user, new TagUsage("family", dining, 4));
        _transactions.Add(user, new TagUsage("family", travel, 4));
        _transactions.Add(user, new TagUsage("date", dining, 6));

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(user, groceries), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal("costco", "family", "date");
    }

    [Fact]
    public async Task HandleAsync_BreaksTiesAlphabetically()
    {
        var user = AUser();
        var groceries = AnId();
        _transactions.Add(user, new TagUsage("weekly", groceries, 2));
        _transactions.Add(user, new TagUsage("bulk", groceries, 2));

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(user, groceries), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal("bulk", "weekly");
    }

    [Fact]
    public async Task HandleAsync_SuggestsTheMostUsedTags_WhenNoCategoryIsChosen()
    {
        var user = AUser();
        _transactions.Add(user, new TagUsage("costco", AnId(), 3));
        _transactions.Add(user, new TagUsage("family", AnId(), 8));

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(user, null), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal("family", "costco");
    }

    [Fact]
    public async Task HandleAsync_SuggestsNothing_WhenTheUserHasNoTags()
    {
        _transactions.Add(AUser(), new TagUsage("costco", AnId(), 3));

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(AUser(), null), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenTheTagsCannotBeRead()
    {
        var error = new UnexpectedError("transactions-read-failed", "Transactions could not be read from the database.");
        _transactions.FailWith(error);

        var result = await _useCase.HandleAsync(new GetTagSuggestionsRequest(AUser(), null), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
