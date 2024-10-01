using Holefeeder.Tests.Common.Builders.Accounts;

using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest, Category("Application")]
public class ModifyAccountTests
{
    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenValidationError()
    {
        // arrange
        var request = ModifyAccountRequestBuilder.GivenAnInvalidModifyAccountRequest().Build();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }
}
