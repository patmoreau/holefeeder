using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core.ScheduleImport;

internal interface IImportQueue
{
    Result<Nothing> Enqueue(Id importId, ImportDataRequest request);
}
