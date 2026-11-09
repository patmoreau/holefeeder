using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Api.CreateStoreItem;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;

namespace Holefeeder.ServiceTests.StoreItems.Api.CreateStoreItem;

public class CreateStoreItemRequestRawTests
{
    [Fact]
    public void ToCreateStoreItemRequest_ReturnsTheCreationForTheUser_WhenValid()
    {
        var user = AUser();

        var result = CreateStoreItemRequestRaw.ToCreateStoreItemRequest(new("settings", "new-data"), user);

        result.Should().BeSuccessful()
            .And.WithValue(new CreateStoreItemRequest(user, Code.Valid("settings"), Data.Valid("new-data")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToCreateStoreItemRequest_FailsValidation_WhenCodeIsEmpty(string? codeRaw)
    {
        var result = CreateStoreItemRequestRaw.ToCreateStoreItemRequest(new(codeRaw, "new-data"), AUser());

        ValidationErrorsOf(result).Should().ContainKey(CodeErrors.CodeInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToCreateStoreItemRequest_FailsValidation_WhenDataIsEmpty(string? dataRaw)
    {
        var result = CreateStoreItemRequestRaw.ToCreateStoreItemRequest(new("settings", dataRaw), AUser());

        ValidationErrorsOf(result).Should().ContainKey(DataErrors.CodeInvalid);
    }

    [Fact]
    public void ToCreateStoreItemRequest_ReportsEveryError_WhenCodeAndDataAreInvalid()
    {
        var result = CreateStoreItemRequestRaw.ToCreateStoreItemRequest(new(null, null), AUser());

        ValidationErrorsOf(result).Should().ContainKeys(CodeErrors.CodeInvalid, DataErrors.CodeInvalid);
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<CreateStoreItemRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
