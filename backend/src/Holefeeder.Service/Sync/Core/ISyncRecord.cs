using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal interface ISyncRecord
{
    Id Id { get; }

    SyncTable Table { get; }
}

internal interface ISyncPatch
{
    SyncTable Table { get; }

    Result<ISyncRecord> Create(Id id);

    Result<ISyncRecord> ApplyTo(ISyncRecord existing);

    ISyncRecord? KeptOnDelete(ISyncRecord existing);
}
