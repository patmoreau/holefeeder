using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Api.ModifyStoreItem;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;

namespace Holefeeder.ServiceTests.StoreItems.Api.ModifyStoreItem;

public class ModifyStoreItemRequestRawTests
{
    [Fact]
    public void ToModifyStoreItemRequest_ReturnsTheModificationForTheUser_WhenValid()
    {
        var user = AUser();
        var id = AnId();

        var result = ModifyStoreItemRequestRaw.ToModifyStoreItemRequest(new(id.Value.ToString(), "new-data"), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyStoreItemRequest(user, id, Data.Valid("new-data")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void ToModifyStoreItemRequest_FailsValidation_WhenIdIsInvalid(string? idRaw)
    {
        var result = ModifyStoreItemRequestRaw.ToModifyStoreItemRequest(new(idRaw, "new-data"), AUser());

        ValidationErrorsOf(result).Should().ContainKey(IdErrors.CodeInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToModifyStoreItemRequest_FailsValidation_WhenDataIsEmpty(string? dataRaw)
    {
        var result = ModifyStoreItemRequestRaw.ToModifyStoreItemRequest(new(AnId().Value.ToString(), dataRaw), AUser());

        ValidationErrorsOf(result).Should().ContainKey(DataErrors.CodeInvalid);
    }

    [Fact]
    public void ToModifyStoreItemRequest_ReportsEveryError_WhenIdAndDataAreInvalid()
    {
        var result = ModifyStoreItemRequestRaw.ToModifyStoreItemRequest(new(null, null), AUser());

        ValidationErrorsOf(result).Should().ContainKeys(IdErrors.CodeInvalid, DataErrors.CodeInvalid);
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<ModifyStoreItemRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
