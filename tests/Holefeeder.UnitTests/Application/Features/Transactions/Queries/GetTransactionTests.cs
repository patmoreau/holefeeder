using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Queries.GetTransaction;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Queries;

[UnitTest, Category("Application")]
public class GetTransactionTests
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
