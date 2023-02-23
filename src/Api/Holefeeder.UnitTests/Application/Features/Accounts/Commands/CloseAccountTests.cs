using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

public class CloseAccountTests
{
    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenValidationError()
    {
        // arrange
        Request request = GivenACloseAccountRequest().WithNoId().Build();

        Validator validator = new Validator();

        // act
        TestValidationResult<Request>? result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
