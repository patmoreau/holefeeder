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

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.TransactionId).GreaterThan(0);
            RuleForEach(command => command.Operations)
                .ChildRules(operation =>
                {
                    operation.RuleFor(op => op.Type)
                        .NotEmpty()
                        .Must(type => type is "accounts" or "categories" or "cashflows" or "store_items" or "transactions")
                        .WithMessage("Table must be one of: accounts, categories, cashflows, store_items or transactions");

                    operation.RuleFor(op => op.Op)
                        .NotEmpty()
                        .Must(op => op is "PUT" or "PATCH" or "DELETE")
                        .WithMessage("Operation must be one of: PUT, PATCH, or DELETE");

                    operation.RuleFor(op => op.Id)
                        .NotEmpty()
                        .WithMessage("Id cannot be empty");
                });
        }
    }
}
