using static Holefeeder.Application.Features.Accounts.Queries.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

[UnitTest, Category("Application")]
public class GetAccountTests
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
