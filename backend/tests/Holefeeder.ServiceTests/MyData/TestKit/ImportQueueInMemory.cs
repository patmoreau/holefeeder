using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.MyData.Core.ScheduleImport;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.MyData.TestKit;

internal sealed class ImportQueueInMemory : IImportQueue
{
    public List<(Id ImportId, ImportDataRequest Request)> Queued { get; } = [];

    public Result<Nothing> Enqueue(Id importId, ImportDataRequest request)
    {
        Queued.Add((importId, request));
        return Nothing.Value;
    }
}
