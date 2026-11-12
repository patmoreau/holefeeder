using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Api;
using Holefeeder.Service.Categories.Api.ModifyCategory;
using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.ModifyCategory;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Categories.Api.ModifyCategory;

public class ModifyCategoryRequestRawTests
{
    private const string AnIdRaw = "3f2504e0-4f89-11d3-9a0c-0305e82c3311";

    [Fact]
    public void ToModifyCategoryRequest_ReturnsTheChangeForTheUser_WhenValid()
    {
        var user = AUser();

        var result = ModifyCategoryRequestRaw.ToModifyCategoryRequest(
            new(new ModifyCategoryBody(AnIdRaw, "Dining ", "expense", "#FF9500", "200", "false")), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyCategoryRequest(
            user, Id.Valid(Guid.Parse(AnIdRaw)), CategoryName.Valid("Dining"), CategoryType.Expense, CategoryColor.Valid("#FF9500"),
            Money.Valid(200m), Favorite: false));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void ToModifyCategoryRequest_FailsValidation_WhenTheIdIsInvalid(string? idRaw) =>
        ValidationErrorsOf(ModifyCategoryRequestRaw.ToModifyCategoryRequest(
                new(new ModifyCategoryBody(idRaw, "Dining", "expense", "#FF9500", "0", null)), AUser()))
            .Should().ContainKey(IdErrors.CodeInvalid);

    [Fact]
    public void ToModifyCategoryRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(ModifyCategoryRequestRaw.ToModifyCategoryRequest(
                new(new ModifyCategoryBody(null, null, null, null, null, "maybe")), AUser()))
            .Should().ContainKeys(
                IdErrors.CodeInvalid, CategoryNameErrors.CodeInvalid, CategoryRequestErrors.CodeTypeInvalid,
                CategoryColorErrors.CodeInvalid, MoneyErrors.CodeInvalid, CategoryRequestErrors.CodeFavoriteInvalid);

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<ModifyCategoryRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
