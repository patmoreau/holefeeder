using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Api;
using Holefeeder.Service.Cashflows.Api.ClearCashflow;
using Holefeeder.Service.Cashflows.Core.ClearCashflow;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.Cashflows.Api.ClearCashflow;

public class ClearCashflowRequestRawTests
{
    [Fact]
    public void ToClearCashflowRequest_ReturnsTheOccurrenceForTheUser_WhenValid()
    {
        var user = AUser();
        var id = AnId();

        var result = ClearCashflowRequestRaw.ToClearCashflowRequest(new(new ClearCashflowBody(id.Value.ToString(), "2026-11-01")), user);

        result.Should().BeSuccessful().And.WithValue(new ClearCashflowRequest(user, id, new DateOnly(2026, 11, 1)));
    }

    [Fact]
    public void ToClearCashflowRequest_ReportsEveryError_WhenEverythingIsInvalid() =>
        ValidationErrorsOf(ClearCashflowRequestRaw.ToClearCashflowRequest(new(new ClearCashflowBody("nope", "tomorrow")), AUser()))
            .Should().ContainKeys(IdErrors.CodeInvalid, IsoDateErrors.CodeCashflowDateInvalid);

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<ClearCashflowRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
