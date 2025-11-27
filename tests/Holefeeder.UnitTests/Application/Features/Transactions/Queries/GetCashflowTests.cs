using static Holefeeder.Application.Features.Transactions.Queries.GetCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

[UnitTest, Category("Application")]
public class GetCashflowTests
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
