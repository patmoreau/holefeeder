namespace Holefeeder.Service.Tags.Persistence;

internal sealed record TaggedRow
{
    public required Guid Id { get; init; }

    public required string Tags { get; init; }
}
