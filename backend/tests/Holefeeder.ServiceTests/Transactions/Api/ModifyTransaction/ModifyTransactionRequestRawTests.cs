using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api;
using Holefeeder.Service.Transactions.Api.ModifyTransaction;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.ModifyTransaction;

namespace Holefeeder.ServiceTests.Transactions.Api.ModifyTransaction;

public class ModifyTransactionRequestRawTests
{
    [Fact]
    public void ToModifyTransactionRequest_ReturnsTheModification_WhenTheBodyIsValid()
    {
        var user = AUser();
        var id = AnId();
        var accountId = AnId();
        var categoryId = AnId();
        var body = new ModifyTransactionBody(id.Value.ToString(), "2026-09-01", "50.25", "Market", accountId.Value.ToString(),
            categoryId.Value.ToString(), ["Weekly", " family "]);

        var result = ModifyTransactionRequestRaw.ToModifyTransactionRequest(new(body), user);

        result.Should().BeSuccessful().And.WithValue(new ModifyTransactionRequest(
            user, id, new DateOnly(2026, 9, 1), Money.Valid(50.25m), "Market", accountId, categoryId, TagList.Create(["weekly", "family"])));
    }

    [Fact]
    public void ToModifyTransactionRequest_LeavesTheDescriptionOut_WhenNoneIsGiven()
    {
        var body = new ModifyTransactionBody(AnId().Value.ToString(), "2026-09-01", "50", null, AnId().Value.ToString(),
            AnId().Value.ToString(), null);

        var result = ModifyTransactionRequestRaw.ToModifyTransactionRequest(new(body), AUser());

        result.Should().BeSuccessful();
        result.Value.Description.Should().BeNull();
        result.Value.Tags.Should().Be(TagList.Create([]));
    }

    [Fact]
    public void ToModifyTransactionRequest_ReportsEveryInvalidField()
    {
        var body = new ModifyTransactionBody(Guid.Empty.ToString(), "2026-02-30", "-1", null, "not-an-id", null, null);

        var result = ModifyTransactionRequestRaw.ToModifyTransactionRequest(new(body), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo([
            IdErrors.CodeInvalid,
            IsoDateErrors.CodeDateInvalid,
            MoneyErrors.CodeInvalid,
            TransactionRequestErrors.CodeAccountIdInvalid,
            TransactionRequestErrors.CodeCategoryIdInvalid
        ]);
    }
}
