using static Holefeeder.Application.UseCases.PowerSync;
using static Holefeeder.Tests.Common.Builders.PowerSync.SyncRequestBuilder;

namespace Holefeeder.UnitTests.Application.UseCases;

[UnitTest, Category("PowerSync")]
public class PowerSyncTests
{
    [Fact]
    public void GivenValidator_WhenTransactionIdIsMissing_ThenValidationError()
    {
        // arrange
        var request = GivenASyncRequest().WithNoTransactionId().Build();
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.TransactionId);
    }

    [Fact]
    public void GivenValidator_WhenInvalidTable_ThenValidationError()
    {
        // arrange
        var request = GivenASyncRequest().WithInvalidType().Build();
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor("Operations[0].Type");
    }

    [Fact]
    public void GivenValidator_WhenInvalidOperation_ThenValidationError()
    {
        // arrange
        var request = GivenASyncRequest().WithInvalidOperation().Build();
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor("Operations[0].Op");
    }

    [Fact]
    public void GivenValidator_WhenMissingOperationId_ThenValidationError()
    {
        // arrange
        var request = GivenASyncRequest().WithNoId().Build();
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor("Operations[0].Id");
    }
}
