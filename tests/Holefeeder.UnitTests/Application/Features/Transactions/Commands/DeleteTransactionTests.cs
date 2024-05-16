using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest]
public class DeleteTransactionTests
{
    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = GivenAnInvalidDeleteTransactionRequest().Build();

        var validator = new Validator();

        // act
        TestValidationResult<Request>? result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
