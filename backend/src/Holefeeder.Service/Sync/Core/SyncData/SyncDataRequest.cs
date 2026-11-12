using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Sync.Core.SyncData;

internal sealed record SyncDataRequest(User User, IReadOnlyList<SyncOperation> Operations);
