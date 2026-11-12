using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core.SyncData;

internal enum SyncOperationKind
{
    Put,
    Patch,
    Delete
}

internal sealed record SyncOperation(SyncOperationKind Kind, Id Id, ISyncPatch Patch);
