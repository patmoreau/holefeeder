using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Api.DeleteTag;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.DeleteTag;

namespace Holefeeder.ServiceTests.Tags.Api.DeleteTag;

public class DeleteTagRequestRawTests
{
    [Fact]
    public void ToDeleteTagRequest_ReturnsTheDeletionForTheUser_WhenValid()
    {
        var user = AUser();

        var result = DeleteTagRequestRaw.ToDeleteTagRequest(new(new DeleteTagBody(" Car ")), user);

        result.Should().BeSuccessful().And.WithValue(new DeleteTagRequest(user, TagName.Valid("Car")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void ToDeleteTagRequest_FailsValidation_WhenTheTagIsMissing(string? tag)
    {
        var result = DeleteTagRequestRaw.ToDeleteTagRequest(new(new DeleteTagBody(tag)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(TagNameErrors.CodeRequired);
    }
}
