using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Sync.Core;

internal interface ISyncStore
{
    Task<Result<ISyncSession>> BeginAsync(User user, CancellationToken cancellationToken);
}

internal interface ISyncSession : IAsyncDisposable
{
    Task<Result<ISyncRecord>> FindAsync(SyncTable table, Id id, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(ISyncRecord record, CancellationToken cancellationToken);

    Task<Result<Nothing>> DeleteAsync(SyncTable table, Id id, CancellationToken cancellationToken);

    Task<Result<Nothing>> CommitAsync(CancellationToken cancellationToken);
}
