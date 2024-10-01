using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

[UnitTest, Category("Application")]
public class ModifyStoreItemTests
{
    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = GivenAModifyStoreItemRequest().WithNoId().Build();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = GivenAModifyStoreItemRequest().WithNoData().Build();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data);
    }

    [Fact]
    public void GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = GivenAModifyStoreItemRequest().Build();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
