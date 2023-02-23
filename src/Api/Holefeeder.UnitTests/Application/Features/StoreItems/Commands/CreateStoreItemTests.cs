using System.Threading.Tasks;
using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class CreateStoreItemTests
{
    [Fact]
    public async Task GivenValidator_WhenCodeIsEmpty_ThenError()
    {
        // arrange
        Request request = GivenACreateStoreItemRequest().WithNoCode().Build();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code);
    }

    [Fact]
    public async Task GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        Request request = GivenACreateStoreItemRequest().WithNoData().Build();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        Request request = GivenACreateStoreItemRequest().Build();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
