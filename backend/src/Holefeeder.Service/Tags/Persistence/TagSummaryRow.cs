using Holefeeder.Service.Tags.Core.ListTags;

namespace Holefeeder.Service.Tags.Persistence;

internal sealed record TagSummaryRow
{
    public required string Tag { get; init; }

    public required long TransactionCount { get; init; }

    public required long ActiveCashflowCount { get; init; }

    public TagSummary ToTagSummary() => new(Tag, (int)TransactionCount, (int)ActiveCashflowCount);
}
