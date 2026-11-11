using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Api.CancelCashflow;
using Holefeeder.Service.Cashflows.Core.CancelCashflow;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Cashflows.Api.CancelCashflow;

public class CancelCashflowRequestRawTests
{
    [Fact]
    public void ToCancelCashflowRequest_ReturnsTheCashflowForTheUser_WhenValid()
    {
        var user = AUser();
        var id = AnId();

        var result = CancelCashflowRequestRaw.ToCancelCashflowRequest(new(new CancelCashflowBody(id.Value.ToString())), user);

        result.Should().BeSuccessful().And.WithValue(new CancelCashflowRequest(user, id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("nope")]
    public void ToCancelCashflowRequest_FailsValidation_WhenCashflowIdIsInvalid(string? idRaw)
    {
        var result = CancelCashflowRequestRaw.ToCancelCashflowRequest(new(new CancelCashflowBody(idRaw)), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
