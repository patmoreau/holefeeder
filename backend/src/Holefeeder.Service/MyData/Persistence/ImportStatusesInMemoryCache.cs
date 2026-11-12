using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;

using Microsoft.Extensions.Caching.Memory;

namespace Holefeeder.Service.MyData.Persistence;

internal sealed class ImportStatusesInMemoryCache(IMemoryCache cache) : IImportStatuses
{
    private static readonly TimeSpan Expiration = TimeSpan.FromHours(1);

    public void Save(ImportStatus status) => cache.Set(KeyOf(status.ImportId), status, Expiration);

    public Result<ImportStatus> Find(Id importId, Id userId) =>
        cache.TryGetValue(KeyOf(importId), out ImportStatus? status) && status is not null && status.UserId == userId
            ? status
            : ImportStatusErrors.NotFound;

    private static (string, Guid) KeyOf(Id importId) => ("my-data-import", importId.Value);
}
