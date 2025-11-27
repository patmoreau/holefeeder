using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest, Category("Application")]
public class DeleteTransactionTests
{
    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = Guid.Empty;

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r);
    }
}
