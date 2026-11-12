using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.MyData.Api.ImportData;

internal sealed record ImportDataBody(
    [property: JsonConverter(typeof(JsonScalarAsText))] string? UpdateExisting,
    ImportDataSetBody? Data);

internal sealed record ImportDataSetBody(
    IReadOnlyList<ImportAccountBody>? Accounts,
    IReadOnlyList<ImportCategoryBody>? Categories,
    IReadOnlyList<ImportCashflowBody>? Cashflows,
    IReadOnlyList<ImportTransactionBody>? Transactions);

internal sealed record ImportAccountBody(
    string? Id,
    string? Type,
    string? Name,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? OpenBalance,
    string? OpenDate,
    string? Description,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Favorite,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Inactive);

internal sealed record ImportCategoryBody(
    string? Id,
    string? Type,
    string? Name,
    string? Color,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? BudgetAmount,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Favorite,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? System,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Inactive);

internal sealed record ImportCashflowBody(
    string? Id,
    string? EffectiveDate,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Amount,
    string? IntervalType,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Frequency,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Recurrence,
    string? Description,
    IReadOnlyList<string?>? Tags,
    string? CategoryId,
    string? AccountId,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Inactive);

internal sealed record ImportTransactionBody(
    string? Id,
    string? Date,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Amount,
    string? Description,
    IReadOnlyList<string?>? Tags,
    string? CategoryId,
    string? AccountId,
    string? CashflowId,
    string? CashflowDate);
