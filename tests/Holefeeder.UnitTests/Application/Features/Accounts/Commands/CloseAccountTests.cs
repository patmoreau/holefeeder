using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest, Category("Application")]
public class CloseAccountTests
{
    [Fact]
    public void GivenValidator_WhenIdIsMissing_ThenValidationError()
    {
        // arrange
        var request = GivenACloseAccountRequest().WithMissingId().Build();
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
