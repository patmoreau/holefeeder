using static Holefeeder.Application.Features.StoreItems.Queries.GetStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Queries;

[UnitTest, Category("Application")]
public class GetStoreItemTests
{
    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = Guid.Empty;

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r);
    }
}
