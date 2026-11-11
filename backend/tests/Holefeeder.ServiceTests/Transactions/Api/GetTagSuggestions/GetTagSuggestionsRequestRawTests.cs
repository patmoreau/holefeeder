using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api.GetTagSuggestions;
using Holefeeder.Service.Transactions.Core.GetTagSuggestions;

namespace Holefeeder.ServiceTests.Transactions.Api.GetTagSuggestions;

public class GetTagSuggestionsRequestRawTests
{
    [Fact]
    public void ToGetTagSuggestionsRequest_ReadsTheCategory()
    {
        var user = AUser();
        var categoryId = AnId();

        var result = GetTagSuggestionsRequestRaw.ToGetTagSuggestionsRequest(new GetTagSuggestionsRequestRaw(categoryId.Value.ToString()), user);

        result.Should().BeSuccessful().And.WithValue(new GetTagSuggestionsRequest(user, categoryId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToGetTagSuggestionsRequest_HasNoCategory_WhenNoneIsGiven(string? categoryId)
    {
        var user = AUser();

        var result = GetTagSuggestionsRequestRaw.ToGetTagSuggestionsRequest(new GetTagSuggestionsRequestRaw(categoryId), user);

        result.Should().BeSuccessful().And.WithValue(new GetTagSuggestionsRequest(user, null));
    }

    [Theory]
    [InlineData("groceries")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void ToGetTagSuggestionsRequest_Fails_WhenTheCategoryIsNotAnId(string categoryId)
    {
        var result = GetTagSuggestionsRequestRaw.ToGetTagSuggestionsRequest(new GetTagSuggestionsRequestRaw(categoryId), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
