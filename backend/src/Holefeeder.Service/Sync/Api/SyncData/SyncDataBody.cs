using System.Text.Json;
using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Sync.Api.SyncData;

internal sealed record SyncDataBody(
    [property: JsonPropertyName("transaction_id"), JsonConverter(typeof(JsonScalarAsText))] string? TransactionId,
    [property: JsonPropertyName("operations")] IReadOnlyList<SyncOperationBody>? Operations);

internal sealed record SyncOperationBody(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("op")] string? Op,
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("data")] IReadOnlyDictionary<string, JsonElement>? Data);
