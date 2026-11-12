using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.MyData.TestKit;

internal sealed class ImportStatusesInMemory : IImportStatuses
{
    private readonly Dictionary<Id, ImportStatus> _statuses = [];

    public List<ImportStatus> History { get; } = [];

    public void Save(ImportStatus status)
    {
        _statuses[status.ImportId] = status;
        History.Add(status);
    }

    public Result<ImportStatus> Find(Id importId, Id userId) =>
        _statuses.TryGetValue(importId, out var status) && status.UserId == userId ? status : ImportStatusErrors.NotFound;
}
