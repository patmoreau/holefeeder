using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Api.GetStoreItem;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;

namespace Holefeeder.ServiceTests.StoreItems.Api.GetStoreItem;

public class GetStoreItemRequestRawTests
{
    [Fact]
    public void ToGetStoreItemRequest_ReturnsTheIdForTheUser_WhenIdIsValid()
    {
        var user = AUser();
        var id = AnId();

        var result = GetStoreItemRequestRaw.ToGetStoreItemRequest(new(id.Value.ToString()), user);

        result.Should().BeSuccessful().And.WithValue(new GetStoreItemRequest(user, id));
    }

    [Fact]
    public void ToGetStoreItemRequest_FailsValidation_WhenIdIsEmpty()
    {
        var result = GetStoreItemRequestRaw.ToGetStoreItemRequest(new(Guid.Empty.ToString()), AUser());

        ValidationErrorsOf(result).Should().ContainKey(IdErrors.CodeInvalid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-guid")]
    public void ToGetStoreItemRequest_FailsValidation_WhenIdIsNotAGuid(string idRaw)
    {
        var result = GetStoreItemRequestRaw.ToGetStoreItemRequest(new(idRaw), AUser());

        ValidationErrorsOf(result).Should().ContainKey(IdErrors.CodeInvalid);
    }

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<GetStoreItemRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
