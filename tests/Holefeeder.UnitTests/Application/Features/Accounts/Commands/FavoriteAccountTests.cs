using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.FavoriteAccountRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest, Category("Application")]
public class FavoriteAccountTests
{
    [Fact]
    public void GivenValidator_WhenIdIsMissing_ThenValidationError()
    {
        // arrange
        var request = GivenAFavoriteAccountRequest().WithMissingId().Build();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public void GivenValidator_WhenValidRequest_ThenNoValidationError()
    {
        // arrange
        var request = GivenAFavoriteAccountRequest().Build();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
