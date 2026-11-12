using System.Text.Json.Serialization;

namespace Holefeeder.Application.UseCases;

public static class PowerSync
{
    public record Request
    {
        [JsonPropertyName("transaction_id")] public long TransactionId { get; init; }

        [JsonPropertyName("operations")] public IReadOnlyCollection<SyncOperation> Operations { get; init; } = [];
    }

    public record SyncOperation
    {
        [JsonPropertyName("type")] public string Type { get; init; } = string.Empty;

        [JsonPropertyName("op")] public string Op { get; init; } = string.Empty;

        [JsonPropertyName("id")] public Guid Id { get; init; } = Guid.Empty;

        [JsonPropertyName("data")] public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
    }
}
