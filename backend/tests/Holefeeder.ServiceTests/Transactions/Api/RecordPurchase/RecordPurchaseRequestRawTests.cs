using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Api;
using Holefeeder.Service.Transactions.Api.RecordPurchase;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.RecordPurchase;

namespace Holefeeder.ServiceTests.Transactions.Api.RecordPurchase;

public class RecordPurchaseRequestRawTests
{
    private const string AccountId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
    private const string CategoryId = "3f2504e0-4f89-11d3-9a0c-0305e82c3302";

    private static RecordPurchaseBody ABody(
        string? date = "2026-11-11", string? amount = "42.50", string? description = "Market", string? accountId = AccountId,
        string? categoryId = CategoryId, string?[]? tags = null, RecordPurchaseRepeatBody? repeat = null) =>
        new(date, amount, description, accountId, categoryId, tags ?? ["Weekly", "costco"], repeat);

    [Fact]
    public void ToRecordPurchaseRequest_ReturnsThePurchaseForTheUser()
    {
        var user = AUser();

        var result = RecordPurchaseRequestRaw.ToRecordPurchaseRequest(new(ABody()), user);

        result.Should().BeSuccessful().And.WithValue(new RecordPurchaseRequest(
            user, new DateOnly(2026, 11, 11), Money.Valid(42.50m), "Market", Id.Valid(Guid.Parse(AccountId)),
            Id.Valid(Guid.Parse(CategoryId)), TagList.Create(["weekly", "costco"]), null));
    }

    [Fact]
    public void ToRecordPurchaseRequest_ReadsTheRepeat()
    {
        var result = RecordPurchaseRequestRaw.ToRecordPurchaseRequest(
            new(ABody(repeat: new RecordPurchaseRepeatBody("2026-11-15", "monthly", "2"))), AUser());

        result.Should().BeSuccessful();
        result.Value.Repeat.Should().Be(new Repeat(new DateOnly(2026, 11, 15), IntervalType.Monthly, Frequency.Valid(2)));
    }

    [Fact]
    public void ToRecordPurchaseRequest_DefaultsMissingDescriptionAndTagsToEmpty()
    {
        var body = ABody() with { Description = null, Tags = null };

        var result = RecordPurchaseRequestRaw.ToRecordPurchaseRequest(new(body), AUser());

        result.Should().BeSuccessful();
        result.Value.Description.Should().BeEmpty();
        result.Value.Tags.Should().Be(TagList.Create([]));
    }

    [Fact]
    public void ToRecordPurchaseRequest_Fails_WithEveryInvalidField()
    {
        var body = new RecordPurchaseBody("11/11/2026", "-1", null, "chequing", "", null, new RecordPurchaseRepeatBody("soon", "fortnightly", "0"));

        var result = RecordPurchaseRequestRaw.ToRecordPurchaseRequest(new(body), AUser());

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors.Keys.Should().BeEquivalentTo(
            IsoDateErrors.CodeDateInvalid,
            MoneyErrors.CodeInvalid,
            TransactionRequestErrors.CodeAccountIdInvalid,
            TransactionRequestErrors.CodeCategoryIdInvalid,
            RecordPurchaseRequestErrors.CodeRepeatStartDateInvalid,
            IntervalTypeErrors.CodeInvalid,
            FrequencyErrors.CodeInvalid);
    }
}
