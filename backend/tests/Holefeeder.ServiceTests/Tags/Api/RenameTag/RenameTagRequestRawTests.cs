using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Tags.Api.RenameTag;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.RenameTag;

namespace Holefeeder.ServiceTests.Tags.Api.RenameTag;

public class RenameTagRequestRawTests
{
    [Fact]
    public void ToRenameTagRequest_ReturnsTheRenameForTheUser_WhenValid()
    {
        var user = AUser();

        var result = RenameTagRequestRaw.ToRenameTagRequest(new(new RenameTagBody(" Car ", " Auto ")), user);

        result.Should().BeSuccessful().And.WithValue(new RenameTagRequest(user, TagName.Valid("Car"), TagName.Valid("auto")));
    }

    [Fact]
    public void ToRenameTagRequest_FailsValidation_WhenTheTagIsMissing() =>
        ValidationErrorsOf(RenameTagRequestRaw.ToRenameTagRequest(new(new RenameTagBody(" ", "auto")), AUser()))
            .Should().ContainKey(TagNameErrors.CodeRequired);

    [Fact]
    public void ToRenameTagRequest_FailsValidation_WhenTheNewNameHoldsAComma() =>
        ValidationErrorsOf(RenameTagRequestRaw.ToRenameTagRequest(new(new RenameTagBody("car", "auto,moto")), AUser()))
            .Should().ContainKey(TagNameErrors.CodeInvalid);

    [Fact]
    public void ToRenameTagRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(RenameTagRequestRaw.ToRenameTagRequest(new(new RenameTagBody(null, null)), AUser()))
            .Should().ContainKeys(TagNameErrors.CodeRequired, TagNameErrors.CodeInvalid);

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<RenameTagRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
