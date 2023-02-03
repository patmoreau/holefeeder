using System.Threading.Tasks;

using static Holefeeder.Application.Features.Transactions.Commands.CancelCashflow;
using static Holefeeder.Tests.Common.Builders.Transactions.CancelCashflowRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class CancelCashflowTests
{
    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = GivenAnInvalidCancelCashflowRequest().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
