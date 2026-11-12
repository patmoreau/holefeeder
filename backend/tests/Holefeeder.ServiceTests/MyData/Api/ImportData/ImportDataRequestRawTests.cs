using System.Text.Json;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Api.ImportData;
using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.MyData.Api.ImportData;

public class ImportDataRequestRawTests
{
    private static readonly Guid AccountId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly Guid CashflowId = Guid.NewGuid();
    private static readonly Guid TransactionId = Guid.NewGuid();

    [Fact]
    public void ToImportDataRequest_ReadsEveryTable()
    {
        var user = AUser();

        var result = Parse($$"""
            {
              "updateExisting": true,
              "data": {
                "accounts": [{ "id": "{{AccountId}}", "type": "creditCard", "name": "Visa", "openBalance": 12.5, "openDate": "2021-02-03",
                               "description": "Travel", "favorite": true, "inactive": false }],
                "categories": [{ "id": "{{CategoryId}}", "type": "gain", "name": "Salary", "color": "#00FF00", "budgetAmount": 2500,
                                 "favorite": true, "system": false, "inactive": false }],
                "cashflows": [{ "id": "{{CashflowId}}", "effectiveDate": "2024-01-15", "amount": 1500, "intervalType": "onetime",
                                "frequency": 1, "recurrence": 0, "description": "Bonus", "tags": ["Work", "yearly"],
                                "categoryId": "{{CategoryId}}", "accountId": "{{AccountId}}", "inactive": false }],
                "transactions": [{ "id": "{{TransactionId}}", "date": "2024-05-17", "amount": 42.99, "description": "Lunch",
                                   "tags": ["food"], "categoryId": "{{CategoryId}}", "accountId": "{{AccountId}}",
                                   "cashflowId": "{{CashflowId}}", "cashflowDate": "2024-01-15" }]
              }
            }
            """, user);

        result.Should().BeSuccessful();
        result.Value.User.Should().Be(user);
        result.Value.UpdateExisting.Should().BeTrue();
        result.Value.Data.Should().BeEquivalentTo(new MyDataSet(
            [new MyDataAccount(Id.Valid(AccountId), AccountType.CreditCard, "Visa", Money.Valid(12.5m), new DateOnly(2021, 2, 3), "Travel",
                Favorite: true, Inactive: false)],
            [new MyDataCategory(Id.Valid(CategoryId), CategoryType.Gain, "Salary", "#00FF00", Money.Valid(2500m), Favorite: true,
                System: false, Inactive: false)],
            [new MyDataCashflow(Id.Valid(CashflowId), new DateOnly(2024, 1, 15), Money.Valid(1500m), IntervalType.OneTime, 1, 0, "Bonus",
                TagList.Create(["work", "yearly"]), Id.Valid(CategoryId), Id.Valid(AccountId), Inactive: false)],
            [new MyDataTransaction(Id.Valid(TransactionId), new DateOnly(2024, 5, 17), Money.Valid(42.99m), "Lunch",
                TagList.Create(["food"]), Id.Valid(CategoryId), Id.Valid(AccountId), Id.Valid(CashflowId), new DateOnly(2024, 1, 15))]));
    }

    [Fact]
    public void ToImportDataRequest_DoesNotUpdateExisting_WhenNotAsked()
    {
        var result = Parse(WithAccount("""{ "name": "Visa" }"""));

        result.Should().BeSuccessful();
        result.Value.UpdateExisting.Should().BeFalse();
    }

    [Fact]
    public void ToImportDataRequest_DefaultsAbsentOptionalFields()
    {
        var result = Parse(WithAccount("""{ "name": "Visa" }"""));

        result.Should().BeSuccessful();
        result.Value.Data.Accounts.Single().Should().Be(new MyDataAccount(Id.Valid(AccountId), AccountType.Checking, "Visa",
            Money.Valid(0m), new DateOnly(2020, 1, 1), string.Empty, Favorite: false, Inactive: false));
    }

    [Theory]
    [InlineData("""{ "data": null }""")]
    [InlineData("""{ "data": { "accounts": [], "categories": [], "cashflows": [], "transactions": [] } }""")]
    [InlineData("{}")]
    public void ToImportDataRequest_FailsValidation_WhenThereIsNoData(string json) =>
        ValidationErrorsOf(Parse(json)).Should().ContainKey(ImportDataErrors.CodeDataEmpty);

    [Theory]
    [InlineData("""{ "id": "not-an-id" }""", "accounts[0].id")]
    [InlineData("""{ "type": "wallet" }""", "accounts[0].type")]
    [InlineData("""{ "openBalance": -1 }""", "accounts[0].openBalance")]
    [InlineData("""{ "openBalance": 1.234 }""", "accounts[0].openBalance")]
    [InlineData("""{ "openDate": "2021-13-45" }""", "accounts[0].openDate")]
    [InlineData("""{ "favorite": "yes" }""", "accounts[0].favorite")]
    public void ToImportDataRequest_NamesTheField_WhenAFieldIsMalformed(string account, string field) =>
        ValidationErrorsOf(Parse(WithAccount(account)))[ImportDataErrors.CodeFieldInvalid]
            .Should().ContainSingle().Which.Should().StartWith(field);

    [Theory]
    [InlineData("categories", """{ "type": "income" }""", "categories[0].type")]
    [InlineData("categories", """{ "type": "1" }""", "categories[0].type")]
    [InlineData("cashflows", """{ "intervalType": "hourly" }""", "cashflows[0].intervalType")]
    [InlineData("cashflows", """{ "frequency": "often" }""", "cashflows[0].frequency")]
    [InlineData("cashflows", """{ "accountId": "" }""", "cashflows[0].accountId")]
    [InlineData("transactions", """{ "cashflowId": "nope" }""", "transactions[0].cashflowId")]
    [InlineData("transactions", """{ "cashflowDate": "yesterday" }""", "transactions[0].cashflowDate")]
    public void ToImportDataRequest_NamesTheFieldOfEachTable_WhenAFieldIsMalformed(string table, string overrides, string field) =>
        ValidationErrorsOf(Parse(WithRecord(table, overrides)))[ImportDataErrors.CodeFieldInvalid]
            .Should().ContainSingle().Which.Should().StartWith(field);

    [Theory]
    [InlineData("accounts", """{ "name": " " }""", MyDataRecordErrors.CodeAccountNameInvalid)]
    [InlineData("categories", """{ "name": "" }""", MyDataRecordErrors.CodeCategoryNameInvalid)]
    [InlineData("cashflows", """{ "frequency": 0 }""", MyDataRecordErrors.CodeCashflowFrequencyInvalid)]
    [InlineData("cashflows", """{ "recurrence": -1 }""", MyDataRecordErrors.CodeCashflowRecurrenceInvalid)]
    public void ToImportDataRequest_FailsValidation_WhenARecordBreaksARule(string table, string overrides, string code) =>
        ValidationErrorsOf(Parse(WithRecord(table, overrides))).Should().ContainKey(code);

    [Fact]
    public void ToImportDataRequest_FailsValidation_WhenUpdateExistingIsMalformed() =>
        ValidationErrorsOf(Parse(WithAccount("{}").Replace("{\"data\"", "{\"updateExisting\":\"always\",\"data\"", StringComparison.Ordinal)))
            [ImportDataErrors.CodeFieldInvalid].Should().ContainSingle().Which.Should().StartWith("updateExisting");

    [Fact]
    public void ToImportDataRequest_ReportsEveryInvalidRecord() =>
        ValidationErrorsOf(Parse(WithRecord("accounts", """{ "openDate": "bad" }""", """{ "openDate": "worse" }""")))
            [ImportDataErrors.CodeFieldInvalid].Should().BeEquivalentTo(
                ImportDataErrors.FieldInvalid("accounts[0].openDate", "a date as YYYY-MM-DD").Description,
                ImportDataErrors.FieldInvalid("accounts[1].openDate", "a date as YYYY-MM-DD").Description);

    private static Result<ImportDataRequest> Parse(string json, Service.Support.Core.User? user = null) =>
        ImportDataRequestRaw.ToImportDataRequest(new ImportDataRequestRaw(JsonSerializer.Deserialize<ImportDataBody>(json,
            JsonSerializerOptions.Web)!), user ?? AUser());

    private static string WithAccount(string overrides) => WithRecord("accounts", overrides);

    private static string WithRecord(string table, params string[] overrides)
    {
        var records = overrides.Select(json =>
        {
            var record = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ValidRecord(table))!;
            foreach (var (key, value) in JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!)
            {
                record[key] = value;
            }
            return record;
        });
        return JsonSerializer.Serialize(new Dictionary<string, object> { ["data"] = new Dictionary<string, object> { [table] = records } });
    }

    private static string ValidRecord(string table) =>
        table switch
        {
            "accounts" => $$"""{ "id": "{{AccountId}}", "type": "checking", "name": "Chequing", "openDate": "2020-01-01" }""",
            "categories" => $$"""{ "id": "{{CategoryId}}", "type": "expense", "name": "Groceries", "budgetAmount": 0 }""",
            "cashflows" => $$"""
                { "id": "{{CashflowId}}", "effectiveDate": "2024-01-15", "amount": 10, "intervalType": "monthly", "frequency": 1,
                  "recurrence": 0, "categoryId": "{{CategoryId}}", "accountId": "{{AccountId}}" }
                """,
            _ => $$"""
                { "id": "{{TransactionId}}", "date": "2024-05-17", "amount": 10, "categoryId": "{{CategoryId}}",
                  "accountId": "{{AccountId}}" }
                """
        };

    private static IReadOnlyDictionary<string, string[]> ValidationErrorsOf(Result<ImportDataRequest> result)
    {
        result.Should().BeFailure();
        return result.Error.Should().BeOfType<ResultErrorAggregate>().Subject.Errors;
    }
}
