using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Api.DeleteCategory;
using Holefeeder.Service.Categories.Core.DeleteCategory;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Categories.Api.DeleteCategory;

public class DeleteCategoryRequestRawTests
{
    [Fact]
    public void ToDeleteCategoryRequest_ReturnsTheCategoryForTheUser_WhenIdIsValid()
    {
        var user = AUser();
        var id = AnId();

        var result = DeleteCategoryRequestRaw.ToDeleteCategoryRequest(new(id.Value.ToString()), user);

        result.Should().BeSuccessful().And.WithValue(new DeleteCategoryRequest(user, id));
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("")]
    [InlineData("not-a-guid")]
    public void ToDeleteCategoryRequest_FailsValidation_WhenIdIsInvalid(string idRaw)
    {
        var result = DeleteCategoryRequestRaw.ToDeleteCategoryRequest(new(idRaw), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
