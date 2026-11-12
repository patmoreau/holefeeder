using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Api;
using Holefeeder.Service.Categories.Api.CreateCategory;
using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.CreateCategory;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Categories.Api.CreateCategory;

public class CreateCategoryRequestRawTests
{
    [Fact]
    public void ToCreateCategoryRequest_ReturnsTheCreationForTheUser_WhenValid()
    {
        var user = AUser();

        var result = CreateCategoryRequestRaw.ToCreateCategoryRequest(
            new(new CreateCategoryBody(" Salary ", "gain", "#30B0C7", "1250.50", "true")), user);

        result.Should().BeSuccessful().And.WithValue(new CreateCategoryRequest(
            user, CategoryName.Valid("Salary"), CategoryType.Gain, CategoryColor.Valid("#30B0C7"), Money.Valid(1250.50m), Favorite: true));
    }

    [Fact]
    public void ToCreateCategoryRequest_IsNotAFavorite_WhenFavoriteIsMissing()
    {
        var result = CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(favorite: null)), AUser());

        result.Should().BeSuccessful();
        result.Value.Favorite.Should().BeFalse();
    }

    [Theory]
    [InlineData("expense", "Expense")]
    [InlineData("Gain", "Gain")]
    public void ToCreateCategoryRequest_ReadsTheType(string typeRaw, string expected)
    {
        var result = CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(type: typeRaw)), AUser());

        result.Should().BeSuccessful();
        result.Value.Type.ToString().Should().Be(expected);
    }

    [Fact]
    public void ToCreateCategoryRequest_FailsValidation_WhenTheNameIsEmpty() =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(name: " ")), AUser()))
            .Should().ContainKey(CategoryNameErrors.CodeInvalid);

    [Theory]
    [InlineData(null)]
    [InlineData("transfer")]
    [InlineData("0")]
    public void ToCreateCategoryRequest_FailsValidation_WhenTheTypeIsUnknown(string? typeRaw) =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(type: typeRaw)), AUser()))
            .Should().ContainKey(CategoryRequestErrors.CodeTypeInvalid);

    [Fact]
    public void ToCreateCategoryRequest_FailsValidation_WhenTheColorIsInvalid() =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(color: "green")), AUser()))
            .Should().ContainKey(CategoryColorErrors.CodeInvalid);

    [Theory]
    [InlineData(null)]
    [InlineData("-1")]
    [InlineData("12.345")]
    public void ToCreateCategoryRequest_FailsValidation_WhenTheBudgetIsInvalid(string? budgetRaw) =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(budget: budgetRaw)), AUser()))
            .Should().ContainKey(MoneyErrors.CodeInvalid);

    [Fact]
    public void ToCreateCategoryRequest_FailsValidation_WhenFavoriteIsNotABoolean() =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(ABody(favorite: "yes")), AUser()))
            .Should().ContainKey(CategoryRequestErrors.CodeFavoriteInvalid);

    [Fact]
    public void ToCreateCategoryRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(CreateCategoryRequestRaw.ToCreateCategoryRequest(new(new CreateCategoryBody(null, null, null, null, "maybe")), AUser()))
            .Should().ContainKeys(
                CategoryNameErrors.CodeInvalid, CategoryRequestErrors.CodeTypeInvalid, CategoryColorErrors.CodeInvalid,
                MoneyErrors.CodeInvalid, CategoryRequestErrors.CodeFavoriteInvalid);

    private static CreateCategoryBody ABody(
        string? name = "Dining", string? type = "expense", string? color = "#FF9500", string? budget = "200.00", string? favorite = "false") =>
        new(name, type, color, budget, favorite);

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<CreateCategoryRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
