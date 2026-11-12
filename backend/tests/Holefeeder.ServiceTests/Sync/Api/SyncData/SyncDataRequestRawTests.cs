using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Sync.Api.SyncData;
using Holefeeder.Service.Sync.Core;
using Holefeeder.Service.Sync.Core.SyncData;

namespace Holefeeder.ServiceTests.Sync.Api.SyncData;

public class SyncDataRequestRawTests
{
    private static readonly Guid AccountId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void ToSyncDataRequest_ReturnsNoOperations_WhenThereAreNone()
    {
        var user = AUser();

        var result = SyncDataRequestRaw.ToSyncDataRequest(new(new SyncDataBody("7", null)), user);

        result.Should().BeSuccessful();
        result.Value.User.Should().Be(user);
        result.Value.Operations.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData("seven")]
    public void ToSyncDataRequest_FailsValidation_WhenTheTransactionIdIsNotPositive(string? transactionId) =>
        ValidationErrorsOf(SyncDataRequestRaw.ToSyncDataRequest(new(new SyncDataBody(transactionId, [])), AUser()))
            .Should().ContainKey(SyncDataErrors.CodeTransactionIdInvalid);

    [Theory]
    [InlineData(null)]
    [InlineData("budgets")]
    [InlineData("Accounts")]
    public void ToSyncDataRequest_FailsValidation_WhenTheTypeIsUnknown(string? type) =>
        ValidationErrorsOf(Parse(AnOperation(type: type)))
            .Should().ContainKey(SyncDataErrors.CodeTypeInvalid);

    [Theory]
    [InlineData(null)]
    [InlineData("MERGE")]
    [InlineData("put")]
    public void ToSyncDataRequest_FailsValidation_WhenTheOperationIsUnknown(string? op) =>
        ValidationErrorsOf(Parse(AnOperation(op: op)))
            .Should().ContainKey(SyncDataErrors.CodeOperationInvalid);

    [Theory]
    [InlineData("")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("not-an-id")]
    public void ToSyncDataRequest_FailsValidation_WhenTheIdIsInvalid(string id) =>
        ValidationErrorsOf(Parse(AnOperation(id: id)))
            .Should().ContainKey(IdErrors.CodeInvalid);

    [Theory]
    [InlineData("PUT", "Put")]
    [InlineData("PATCH", "Patch")]
    [InlineData("DELETE", "Delete")]
    public void ToSyncDataRequest_ReadsTheOperationKind(string op, string kind)
    {
        var expected = Enum.Parse<SyncOperationKind>(kind);
        var id = Guid.NewGuid();

        var result = Parse(AnOperation("store_items", op, id.ToString()));

        result.Should().BeSuccessful();
        result.Value.Operations.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Kind = expected, Id = Id.Valid(id) });
    }

    [Fact]
    public void ToSyncDataRequest_IgnoresTheData_WhenDeleting()
    {
        var result = Parse(AnOperation("accounts", "DELETE", data: """{ "open_date": "not a date" }"""));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncAccountPatch());
    }

    [Fact]
    public void ToSyncDataRequest_ReadsAnAccountPatch()
    {
        var result = Parse(AnOperation("accounts", data: """
            {
              "type": "creditcard", "name": "Visa", "open_balance": 12345, "open_date": "2024-02-29",
              "description": "Travel", "favorite": 1, "inactive": 0
            }
            """));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncAccountPatch(
            AccountType.CreditCard, "Visa", Money.Valid(123.45m), new DateOnly(2024, 2, 29), "Travel", true, false));
    }

    [Fact]
    public void ToSyncDataRequest_LeavesAbsentAndNullFieldsUnset()
    {
        var result = Parse(AnOperation("accounts", data: """{ "name": null }"""));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncAccountPatch());
    }

    [Fact]
    public void ToSyncDataRequest_ReadsACategoryPatch()
    {
        var result = Parse(AnOperation("categories", data: """
            {
              "type": "gain", "name": "Salary", "color": "#00FF00", "budget_amount": 250000,
              "favorite": true, "system": 0, "inactive": false
            }
            """));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncCategoryPatch(
            CategoryType.Gain, "Salary", "#00FF00", Money.Valid(2500m), true, false, false));
    }

    [Fact]
    public void ToSyncDataRequest_ReadsAStoreItemPatch()
    {
        var result = Parse(AnOperation("store_items", data: """{ "code": "settings", "data": "{\"a\":1}" }"""));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncStoreItemPatch("settings", "{\"a\":1}"));
    }

    [Fact]
    public void ToSyncDataRequest_ReadsATransactionPatch()
    {
        var cashflowId = Guid.NewGuid();

        var result = Parse(AnOperation("transactions", data: $$"""
            {
              "date": "2024-05-17", "amount": 4299, "description": "Lunch", "account_id": "{{AccountId}}",
              "category_id": "{{CategoryId}}", "cashflow_id": "{{cashflowId}}", "cashflow_date": "2024-05-01",
              "tags": "Food, work,food"
            }
            """));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncTransactionPatch(
            new DateOnly(2024, 5, 17), Money.Valid(42.99m), "Lunch", Id.Valid(AccountId), Id.Valid(CategoryId), Id.Valid(cashflowId),
            new DateOnly(2024, 5, 1), TagList.Create(["food", "work"])));
    }

    [Fact]
    public void ToSyncDataRequest_ReadsACashflowPatch()
    {
        var result = Parse(AnOperation("cashflows", data: $$"""
            {
              "effective_date": "2024-01-15", "interval_type": "weekly", "frequency": 2, "recurrence": 0, "amount": 150000,
              "description": "Rent", "account_id": "{{AccountId}}", "category_id": "{{CategoryId}}", "inactive": 0,
              "tags": ["Home", "bills"]
            }
            """));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncCashflowPatch(
            new DateOnly(2024, 1, 15), IntervalType.Weekly, 2, 0, Money.Valid(1500m), "Rent", Id.Valid(AccountId), Id.Valid(CategoryId),
            false, TagList.Create(["home", "bills"])));
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("null")]
    [InlineData("[]")]
    public void ToSyncDataRequest_ClearsTheTags_WhenTheTagsAreEmpty(string tags)
    {
        var result = Parse(AnOperation("transactions", data: $$"""{ "tags": {{tags}} }"""));

        result.Should().BeSuccessful();
        ((SyncTransactionPatch)result.Value.Operations.Single().Patch).Tags.Should().Be(TagList.Create([]));
    }

    [Fact]
    public void ToSyncDataRequest_ReadsNonTextAsItsJson_ForTextFields()
    {
        var result = Parse(AnOperation("store_items", data: """{ "code": "counter", "data": 42 }"""));

        result.Should().BeSuccessful();
        result.Value.Operations.Single().Patch.Should().Be(new SyncStoreItemPatch("counter", "42"));
    }

    [Theory]
    [InlineData("accounts", """{ "open_date": "2024-13-45" }""")]
    [InlineData("accounts", """{ "open_date": 20240101 }""")]
    [InlineData("accounts", """{ "open_balance": 12.5 }""")]
    [InlineData("accounts", """{ "open_balance": -100 }""")]
    [InlineData("accounts", """{ "favorite": "yes" }""")]
    [InlineData("categories", """{ "type": "Income" }""")]
    [InlineData("transactions", """{ "account_id": "not-an-id" }""")]
    [InlineData("transactions", """{ "tags": 12 }""")]
    [InlineData("transactions", """{ "tags": [1, 2] }""")]
    [InlineData("cashflows", """{ "frequency": "often" }""")]
    public void ToSyncDataRequest_FailsValidationOnTheField_WhenTheDataIsMalformed(string type, string data) =>
        ValidationErrorsOf(Parse(AnOperation(type, data: data)))
            .Should().ContainKey(SyncDataErrors.CodeFieldInvalid);

    [Fact]
    public void ToSyncDataRequest_NamesTheField_WhenTheDataIsMalformed() =>
        ValidationErrorsOf(Parse(AnOperation("accounts", data: """{ "open_date": "yesterday" }""")))[SyncDataErrors.CodeFieldInvalid]
            .Should().ContainSingle().Which.Should().Contain("open_date");

    [Theory]
    [InlineData("accounts", """{ "type": "Wallet" }""", AccountTypeErrors.CodeInvalid)]
    [InlineData("cashflows", """{ "interval_type": "Hourly" }""", IntervalTypeErrors.CodeInvalid)]
    public void ToSyncDataRequest_FailsValidation_WhenATypeIsUnknown(string type, string data, string code) =>
        ValidationErrorsOf(Parse(AnOperation(type, data: data))).Should().ContainKey(code);

    [Fact]
    public void ToSyncDataRequest_ReportsEveryInvalidOperation()
    {
        var result = SyncDataRequestRaw.ToSyncDataRequest(
            new(new SyncDataBody("1", [AnOperation(type: "budgets"), AnOperation(op: "MERGE")])), AUser());

        ValidationErrorsOf(result).Keys.Should().BeEquivalentTo([SyncDataErrors.CodeTypeInvalid, SyncDataErrors.CodeOperationInvalid]);
    }

    private static Result<SyncDataRequest> Parse(SyncOperationBody operation) =>
        SyncDataRequestRaw.ToSyncDataRequest(new(new SyncDataBody("1", [operation])), AUser());

    private static SyncOperationBody AnOperation(
        string? type = "accounts", string? op = "PATCH", string? id = null, string data = "{}") =>
        new(type, op, id ?? Guid.NewGuid().ToString(), JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data));

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<SyncDataRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
