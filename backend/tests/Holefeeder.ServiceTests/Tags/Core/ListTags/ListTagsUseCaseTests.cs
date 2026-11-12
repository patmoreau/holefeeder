using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core.ListTags;
using Holefeeder.ServiceTests.Tags.TestKit;

namespace Holefeeder.ServiceTests.Tags.Core.ListTags;

public class ListTagsUseCaseTests
{
    private readonly TagsInMemory _tags = new();
    private readonly ListTagsUseCase _useCase;

    public ListTagsUseCaseTests() => _useCase = new ListTagsUseCase(_tags);

    [Fact]
    public async Task HandleAsync_ListsTheUsersTags()
    {
        var user = AUser();
        var groceries = new TagSummary("groceries", 38, 0);
        var home = new TagSummary("home", 12, 1);
        _tags.Add(user, groceries);
        _tags.Add(user, home);
        _tags.Add(AUser(), new TagSummary("someone-else", 1, 0));

        var result = await _useCase.HandleAsync(new ListTagsRequest(user), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(groceries, home);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenTagsCannotBeRead()
    {
        var failure = new ResultError("tags-unavailable", "Tags are unavailable.");
        _tags.FailWith(failure);

        var result = await _useCase.HandleAsync(new ListTagsRequest(AUser()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
