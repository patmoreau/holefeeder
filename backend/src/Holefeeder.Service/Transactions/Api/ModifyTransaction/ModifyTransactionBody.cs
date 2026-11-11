using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Transactions.Api.ModifyTransaction;

internal sealed record ModifyTransactionBody(
    string? Id,
    string? Date,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Amount,
    string? Description,
    string? AccountId,
    string? CategoryId,
    string?[]? Tags);
