using Holefeeder.Tests.Common.Builders.Accounts;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Commands;

[UnitTest, Category("Application")]
public class OpenAccountTests
{
    [Fact]
    public async Task GivenValidator_WhenNoType_ThenError()
    {
        // arrange
        var request = OpenAccountRequestBuilder.GivenAnOpenAccountRequest()
            .WithNoType()
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GivenValidator_WhenNoName_ThenError(string? name)
    {
        // arrange
        var request = OpenAccountRequestBuilder.GivenAnOpenAccountRequest()
            .WithName(name!)
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    [Fact]
    public async Task GivenValidator_WhenNoOpenDate_ThenError()
    {
        // arrange
        var request = OpenAccountRequestBuilder.GivenAnOpenAccountRequest()
            .WithNoOpenDate()
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.OpenDate);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = OpenAccountRequestBuilder.GivenAnOpenAccountRequest()
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
