using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

[UnitTest, Category("Application")]
public class CreateStoreItemTests
{
    [Fact]
    public async Task GivenValidator_WhenCodeIsEmpty_ThenError()
    {
        // arrange
        var request = GivenACreateStoreItemRequest().WithNoCode().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code);
    }

    [Fact]
    public async Task GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = GivenACreateStoreItemRequest().WithNoData().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = GivenACreateStoreItemRequest().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
