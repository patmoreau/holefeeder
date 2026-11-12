using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.RenameTag;
using Holefeeder.ServiceTests.Tags.TestKit;

namespace Holefeeder.ServiceTests.Tags.Core.RenameTag;

public class RenameTagUseCaseTests
{
    private readonly TagsInMemory _tags = new();
    private readonly RenameTagUseCase _useCase;

    public RenameTagUseCaseTests() => _useCase = new RenameTagUseCase(_tags);

    [Fact]
    public async Task HandleAsync_RenamesTheTagOnTheUsersItems()
    {
        var user = AUser();
        var groceries = _tags.Tagged(user, "car,groceries");
        var trip = _tags.Tagged(user, "trip,car");
        var someoneElse = _tags.Tagged(AUser(), "car");

        var result = await _useCase.HandleAsync(Renaming(user, "car", "auto"), CancellationToken.None);

        result.Should().BeSuccessful();
        _tags.TagsOf(groceries).Should().Be("auto,groceries");
        _tags.TagsOf(trip).Should().Be("trip,auto");
        _tags.TagsOf(someoneElse).Should().Be("car");
    }

    [Fact]
    public async Task HandleAsync_ChangesNothing_WhenTheNameIsTheSame()
    {
        var user = AUser();
        _tags.Tagged(user, "car");
        _tags.FailWith(new ResultError("tags-unavailable", "Tags are unavailable."));

        var result = await _useCase.HandleAsync(Renaming(user, "car", "car"), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenTagsCannotBeWritten()
    {
        var failure = new ResultError("tags-unavailable", "Tags are unavailable.");
        _tags.FailWith(failure);

        var result = await _useCase.HandleAsync(Renaming(AUser(), "car", "auto"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }

    private static RenameTagRequest Renaming(Holefeeder.Service.Support.Core.User user, string tag, string newTag) =>
        new(user, TagName.Valid(tag), TagName.Valid(newTag));
}
