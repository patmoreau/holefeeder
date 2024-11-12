namespace Holefeeder.Ui.Common.Models;

public record MakePurchaseRequest
{
    public required DateOnly Date { get; init; }

    public required decimal Amount { get; init; }

    public string Description { get; init; } = string.Empty;

    public required Guid AccountId { get; init; }

    public required Guid CategoryId { get; init; }

    public IReadOnlyCollection<string> Tags { get; init; } = [];
}
