using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api.RecordTransfer;
using Holefeeder.Service.Transactions.Core.RecordTransfer;

namespace Holefeeder.ServiceTests.Transactions.Api.RecordTransfer;

public class RecordTransferRequestRawTests
{
    private const string FromAccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
    private const string ToAccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3302";

    [Fact]
    public void ToRecordTransferRequest_ReturnsTheTransferForTheUser()
    {
        var user = AUser();

        var result = RecordTransferRequestRaw.ToRecordTransferRequest(
            new(new RecordTransferBody("2026-11-11", "500", null, FromAccountId, ToAccountId)), user);

        result.Should().BeSuccessful().And.WithValue(new RecordTransferRequest(
            user, new DateOnly(2026, 11, 11), Money.Valid(500m), "", Id.Valid(Guid.Parse(FromAccountId)), Id.Valid(Guid.Parse(ToAccountId))));
    }

    [Fact]
    public void ToRecordTransferRequest_Fails_WithEveryInvalidField()
    {
        var result = RecordTransferRequestRaw.ToRecordTransferRequest(
            new(new RecordTransferBody(null, "1.234", "Rainy day", "", "savings")), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            IsoDateErrors.CodeDateInvalid,
            MoneyErrors.CodeInvalid,
            RecordTransferRequestErrors.CodeFromAccountIdInvalid,
            RecordTransferRequestErrors.CodeToAccountIdInvalid);
    }
}
