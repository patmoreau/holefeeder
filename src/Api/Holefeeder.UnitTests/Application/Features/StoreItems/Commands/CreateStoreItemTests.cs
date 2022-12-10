using System.Threading.Tasks;

using AutoBogus;

using Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class CreateStoreItemTests
{
    private readonly AutoFaker<Request> _faker = new();

    [Fact]
    public async Task GivenValidator_WhenCodeIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Code, string.Empty).Generate();

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
        var request = _faker.RuleFor(x => x.Data, string.Empty).Generate();

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
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
