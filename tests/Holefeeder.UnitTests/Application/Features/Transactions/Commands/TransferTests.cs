using static Holefeeder.Application.Features.Transactions.Commands.Transfer;
using static Holefeeder.Tests.Common.Builders.Transactions.TransferRequestBuilder;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

[UnitTest, Category("Application")]
public class TransferTests
{
    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        var request = GivenATransfer().WithNoDate().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenFromAccountIdIsEmpty_ThenError()
    {
        // arrange
        var request = GivenATransfer()
            .WithNoSourceAccount()
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.FromAccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenToAccountIdIsEmpty_ThenError()
    {
        // arrange
        var request = GivenATransfer()
            .WithNoDestinationAccount()
            .Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.ToAccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = GivenATransfer().Build();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
