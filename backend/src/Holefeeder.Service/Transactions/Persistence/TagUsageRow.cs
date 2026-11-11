using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed record TagUsageRow(string Tag, Guid CategoryId, int Count)
{
    public TagUsage ToTagUsage() => new(Tag, Shared.Core.Id.Valid(CategoryId), Count);
}
