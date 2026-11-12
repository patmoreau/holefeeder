using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.DeleteTag;
using Holefeeder.ServiceTests.Tags.TestKit;

namespace Holefeeder.ServiceTests.Tags.Core.DeleteTag;

public class DeleteTagUseCaseTests
{
    private readonly TagsInMemory _tags = new();
    private readonly DeleteTagUseCase _useCase;

    public DeleteTagUseCaseTests() => _useCase = new DeleteTagUseCase(_tags);

    [Fact]
    public async Task HandleAsync_RemovesTheTagFromTheUsersItems()
    {
        var user = AUser();
        var groceries = _tags.Tagged(user, "car,groceries");
        var trip = _tags.Tagged(user, "trip,car");
        var someoneElse = _tags.Tagged(AUser(), "car");

        var result = await _useCase.HandleAsync(new DeleteTagRequest(user, TagName.Valid("car")), CancellationToken.None);

        result.Should().BeSuccessful();
        _tags.TagsOf(groceries).Should().Be("groceries");
        _tags.TagsOf(trip).Should().Be("trip");
        _tags.TagsOf(someoneElse).Should().Be("car");
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenTagsCannotBeWritten()
    {
        var failure = new ResultError("tags-unavailable", "Tags are unavailable.");
        _tags.FailWith(failure);

        var result = await _useCase.HandleAsync(new DeleteTagRequest(AUser(), TagName.Valid("car")), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
